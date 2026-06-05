using System.Text.Json;
using KidsMath.Application.Abstractions;
using KidsMath.Application.Exercise;
using KidsMath.Application.Options;
using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KidsMath.Application.Services;

public class ExerciseSessionService(
    IKidsMathDbContext db,
    SessionQuestionGenerator sessionQuestionGenerator,
    IRandomNumberSource random,
    ProgressService progressService,
    AchievementService achievementService,
    IOptions<ExerciseOptions> exerciseOptions)
{
    public int MaxAttemptsPerQuestion => exerciseOptions.Value.MaxAttemptsPerQuestion;

    public async Task<ExerciseSession?> StartSessionAsync(
        Guid studentId,
        Guid taskDefinitionId,
        int? questionCount,
        CancellationToken ct = default)
    {
        var definition = await db.MathTaskDefinitions.FirstOrDefaultAsync(d => d.Id == taskDefinitionId && d.IsActive, ct);
        var student = await db.StudentProfiles.FirstOrDefaultAsync(s => s.Id == studentId, ct);
        if (definition is null || student is null) return null;

        var count = questionCount ?? exerciseOptions.Value.DefaultQuestionCount;
        var theme = ThemeSelector.PickForStudent(student, random);
        var session = new ExerciseSession
        {
            Id = Guid.NewGuid(),
            StudentProfileId = studentId,
            StartedAtUtc = DateTime.UtcNow,
            Grade = definition.Grade,
            TaskType = definition.TaskType,
            DifficultyLevel = definition.DifficultyLevel,
            TotalQuestions = count,
            CorrectAnswers = 0,
            WrongAnswers = 0,
            Status = SessionStatus.InProgress,
            TemplateThemeId = (int)theme
        };
        db.ExerciseSessions.Add(session);

        var questions = sessionQuestionGenerator.Generate(definition, count);
        for (var i = 0; i < questions.Count; i++)
        {
            var themed = QuestionThemeFormatter.ApplyTheme(theme, questions[i]);
            db.ExerciseAttempts.Add(new ExerciseAttempt
            {
                Id = Guid.NewGuid(),
                ExerciseSessionId = session.Id,
                StudentProfileId = studentId,
                MathTaskDefinitionId = definition.Id,
                QuestionOrder = i + 1,
                GeneratedQuestionJson = JsonSerializer.Serialize(themed.QuestionData, JsonSerializerOptions.Web),
                QuestionTextCs = themed.QuestionTextCs,
                QuestionTextEn = themed.QuestionTextEn,
                TemplateThemeId = (int)themed.Theme,
                CorrectAnswer = themed.CorrectAnswer
            });
        }

        await db.SaveChangesAsync(ct);
        return await GetSessionWithAttemptsAsync(session.Id, ct);
    }

    public async Task<SubmitAnswerResult?> SubmitAnswerAsync(Guid sessionId, Guid attemptId, string answer, CancellationToken ct = default)
    {
        var maxAttempts = exerciseOptions.Value.MaxAttemptsPerQuestion;
        var attempt = await db.ExerciseAttempts
            .Include(a => a.ExerciseSession)
            .Include(a => a.AnswerSubmissions)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.ExerciseSessionId == sessionId, ct);

        if (attempt is null || attempt.ExerciseSession.Status != SessionStatus.InProgress)
        {
            return null;
        }

        if (attempt.IsCorrect != null)
        {
            return null;
        }

        var attemptsUsed = attempt.AnswerSubmissions.Count;
        if (attemptsUsed >= maxAttempts)
        {
            return null;
        }

        var trimmedAnswer = answer.Trim();
        var submissionIsCorrect = AnswerValidator.IsCorrect(attempt.CorrectAnswer, trimmedAnswer);
        var attemptNumber = attemptsUsed + 1;

        db.AnswerSubmissions.Add(new AnswerSubmission
        {
            Id = Guid.NewGuid(),
            ExerciseAttemptId = attempt.Id,
            AttemptNumber = attemptNumber,
            Answer = trimmedAnswer,
            IsCorrect = submissionIsCorrect,
            SubmittedAtUtc = DateTime.UtcNow
        });

        var questionResolved = submissionIsCorrect || attemptNumber >= maxAttempts;
        if (questionResolved)
        {
            attempt.StudentAnswer = trimmedAnswer;
            attempt.IsCorrect = submissionIsCorrect;
            attempt.AnsweredAtUtc = DateTime.UtcNow;

            var session = attempt.ExerciseSession;
            if (submissionIsCorrect)
            {
                session.CorrectAnswers++;
            }
            else
            {
                session.WrongAnswers++;
            }

            await progressService.UpdateAfterQuestionResolvedAsync(attempt, ct);
            await achievementService.EvaluateAfterAnswerAsync(attempt.StudentProfileId, ct);
        }

        await db.SaveChangesAsync(ct);

        return new SubmitAnswerResult
        {
            Attempt = attempt,
            SubmissionIsCorrect = submissionIsCorrect,
            QuestionResolved = questionResolved,
            AttemptsUsed = attemptNumber,
            MaxAttempts = maxAttempts
        };
    }

    public async Task<ExerciseSession?> FinishSessionAsync(Guid sessionId, CancellationToken ct = default)
    {
        var session = await db.ExerciseSessions
            .Include(s => s.Attempts)
            .FirstOrDefaultAsync(s => s.Id == sessionId, ct);

        if (session is null || session.Status != SessionStatus.InProgress) return null;

        session.FinishedAtUtc = DateTime.UtcNow;
        session.Status = SessionStatus.Completed;
        session.CorrectAnswers = session.Attempts.Count(a => a.IsCorrect == true);
        session.WrongAnswers = session.Attempts.Count(a => a.IsCorrect == false);

        await db.SaveChangesAsync(ct);
        await progressService.UpdateAfterSessionAsync(session, ct);
        await achievementService.EvaluateAfterSessionAsync(session, ct);
        return session;
    }

    public async Task<ExerciseSession?> GetSessionWithAttemptsAsync(Guid sessionId, CancellationToken ct = default) =>
        await db.ExerciseSessions
            .AsNoTracking()
            .Include(s => s.Attempts.OrderBy(a => a.QuestionOrder))
                .ThenInclude(a => a.AnswerSubmissions.OrderBy(s => s.AttemptNumber))
            .FirstOrDefaultAsync(s => s.Id == sessionId, ct);

    public async Task<IReadOnlyList<ExerciseSession>> ListSessionsForStudentAsync(Guid studentId, int limit = 20, CancellationToken ct = default) =>
        await db.ExerciseSessions.AsNoTracking()
            .Where(s => s.StudentProfileId == studentId)
            .OrderByDescending(s => s.StartedAtUtc)
            .Take(limit)
            .ToListAsync(ct);
}
