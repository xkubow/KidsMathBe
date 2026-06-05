using KidsMath.Api.Extensions;
using KidsMath.Api.Mapping;
using KidsMath.Application.Services;
using KidsMath.Contracts.Exercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsMath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/exercise-sessions")]
public class ExerciseSessionsController(ExerciseSessionService sessionService, StudentService studentService) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult<ExerciseSessionResponse>> Start(StartSessionRequest request, [FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        if (!await CanAccessStudentAsync(request.StudentProfileId, ct)) return Forbid();
        var session = await sessionService.StartSessionAsync(
            request.StudentProfileId, request.TaskDefinitionId, request.QuestionCount, request.Theme, ct);
        if (session is null) return NotFound();
        return Ok(ExerciseSessionMapper.ToResponse(session, lang, sessionService.MaxAttemptsPerQuestion));
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<ExerciseSessionResponse>> Get(Guid sessionId, [FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        var session = await sessionService.GetSessionWithAttemptsAsync(sessionId, ct);
        if (session is null) return NotFound();
        if (!await CanAccessStudentAsync(session.StudentProfileId, ct)) return Forbid();
        return Ok(ExerciseSessionMapper.ToResponse(session, lang, sessionService.MaxAttemptsPerQuestion, includeAnswers: true));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExerciseSessionListItemResponse>>> ListForStudent([FromQuery] Guid studentId, CancellationToken ct = default)
    {
        if (!await CanAccessStudentAsync(studentId, ct)) return Forbid();
        var sessions = await sessionService.ListSessionsForStudentAsync(studentId, ct: ct);
        return Ok(sessions.Select(ExerciseSessionMapper.ToListItem).ToList());
    }

    [HttpPost("{sessionId:guid}/answer")]
    public async Task<ActionResult<SubmitAnswerResponse>> Answer(Guid sessionId, SubmitAnswerRequest request, CancellationToken ct = default)
    {
        var existing = await sessionService.GetSessionWithAttemptsAsync(sessionId, ct);
        if (existing is null) return NotFound();
        if (!await CanAccessStudentAsync(existing.StudentProfileId, ct)) return Forbid();

        var result = await sessionService.SubmitAnswerAsync(sessionId, request.AttemptId, request.Answer, ct);
        if (result is null) return BadRequest();

        return Ok(ExerciseSessionMapper.ToSubmitAnswerResponse(result));
    }

    [HttpPost("{sessionId:guid}/finish")]
    public async Task<ActionResult<ExerciseSessionResponse>> Finish(Guid sessionId, [FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        var existing = await sessionService.GetSessionWithAttemptsAsync(sessionId, ct);
        if (existing is null) return NotFound();
        if (!await CanAccessStudentAsync(existing.StudentProfileId, ct)) return Forbid();

        var session = await sessionService.FinishSessionAsync(sessionId, ct);
        return Ok(ExerciseSessionMapper.ToResponse(session!, lang, sessionService.MaxAttemptsPerQuestion, includeAnswers: true));
    }

    private async Task<bool> CanAccessStudentAsync(Guid studentId, CancellationToken ct)
    {
        if (User.IsStudentToken()) return User.GetStudentId() == studentId;
        return await studentService.GetForParentAsync(User.GetParentUserId(), studentId, ct) is not null;
    }
}
