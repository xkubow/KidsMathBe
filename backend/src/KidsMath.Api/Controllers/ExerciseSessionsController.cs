using KidsMath.Api.Extensions;
using KidsMath.Application.Services;
using KidsMath.Contracts.Exercise;
using KidsMath.Contracts.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsMath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/exercise-sessions")]
public class ExerciseSessionsController(ExerciseSessionService sessionService, StudentService studentService) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult<object>> Start(StartSessionRequest request, [FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        if (!await CanAccessStudentAsync(request.StudentProfileId, ct)) return Forbid();
        var session = await sessionService.StartSessionAsync(request.StudentProfileId, request.TaskDefinitionId, request.QuestionCount, ct);
        if (session is null) return NotFound();
        return Ok(MapSession(session, lang, sessionService.MaxAttemptsPerQuestion));
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<object>> Get(Guid sessionId, [FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        var session = await sessionService.GetSessionWithAttemptsAsync(sessionId, ct);
        if (session is null) return NotFound();
        if (!await CanAccessStudentAsync(session.StudentProfileId, ct)) return Forbid();
        return Ok(MapSession(session, lang, sessionService.MaxAttemptsPerQuestion, includeAnswers: true));
    }

    [HttpGet]
    public async Task<ActionResult<object>> ListForStudent([FromQuery] Guid studentId, CancellationToken ct = default)
    {
        if (!await CanAccessStudentAsync(studentId, ct)) return Forbid();
        var sessions = await sessionService.ListSessionsForStudentAsync(studentId, ct: ct);
        return Ok(sessions.Select(s => new
        {
            s.Id,
            s.StartedAtUtc,
            s.FinishedAtUtc,
            taskType = s.TaskType.ToString(),
            s.CorrectAnswers,
            s.WrongAnswers,
            s.TotalQuestions,
            status = s.Status.ToString()
        }));
    }

    [HttpPost("{sessionId:guid}/answer")]
    public async Task<ActionResult<object>> Answer(Guid sessionId, SubmitAnswerRequest request, CancellationToken ct = default)
    {
        var existing = await sessionService.GetSessionWithAttemptsAsync(sessionId, ct);
        if (existing is null) return NotFound();
        if (!await CanAccessStudentAsync(existing.StudentProfileId, ct)) return Forbid();

        var result = await sessionService.SubmitAnswerAsync(sessionId, request.AttemptId, request.Answer, ct);
        if (result is null) return BadRequest();

        var attempt = result.Attempt;
        return Ok(new
        {
            attempt.Id,
            isCorrect = result.SubmissionIsCorrect,
            questionResolved = result.QuestionResolved,
            attemptsUsed = result.AttemptsUsed,
            maxAttempts = result.MaxAttempts,
            attempt.StudentAnswer,
            correctAnswer = result.QuestionResolved ? attempt.CorrectAnswer : null,
            finalOutcome = attempt.IsCorrect
        });
    }

    [HttpPost("{sessionId:guid}/finish")]
    public async Task<ActionResult<object>> Finish(Guid sessionId, [FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        var existing = await sessionService.GetSessionWithAttemptsAsync(sessionId, ct);
        if (existing is null) return NotFound();
        if (!await CanAccessStudentAsync(existing.StudentProfileId, ct)) return Forbid();

        var session = await sessionService.FinishSessionAsync(sessionId, ct);
        return Ok(MapSession(session!, lang, sessionService.MaxAttemptsPerQuestion, includeAnswers: true));
    }

    private async Task<bool> CanAccessStudentAsync(Guid studentId, CancellationToken ct)
    {
        if (User.IsStudentToken()) return User.GetStudentId() == studentId;
        return await studentService.GetForParentAsync(User.GetParentUserId(), studentId, ct) is not null;
    }

    private static object MapSession(
        Domain.Entities.ExerciseSession session,
        string lang,
        int maxAttemptsPerQuestion,
        bool includeAnswers = false) => new
    {
        session.Id,
        session.StudentProfileId,
        session.StartedAtUtc,
        session.FinishedAtUtc,
        session.Grade,
        taskType = session.TaskType.ToString(),
        session.DifficultyLevel,
        session.TotalQuestions,
        session.CorrectAnswers,
        session.WrongAnswers,
        status = session.Status.ToString(),
        maxAttemptsPerQuestion,
        attempts = session.Attempts.OrderBy(a => a.QuestionOrder).Select(a =>
        {
            var isResolved = a.IsCorrect != null;
            var submissions = a.AnswerSubmissions.OrderBy(s => s.AttemptNumber).Select(s => new
            {
                s.AttemptNumber,
                s.Answer,
                s.IsCorrect,
                s.SubmittedAtUtc
            }).ToList();

            return new
            {
                a.Id,
                a.QuestionOrder,
                questionText = new LocalizedText(a.QuestionTextCs, a.QuestionTextEn).For(lang),
                a.QuestionTextCs,
                a.QuestionTextEn,
                generatedQuestionJson = a.GeneratedQuestionJson,
                correctAnswer = includeAnswers && isResolved ? a.CorrectAnswer : null,
                studentAnswer = a.StudentAnswer,
                a.IsCorrect,
                a.AnsweredAtUtc,
                isResolved,
                attemptsUsed = submissions.Count,
                maxAttempts = maxAttemptsPerQuestion,
                submissions
            };
        })
    };
}
