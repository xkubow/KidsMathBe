using KidsMath.Application.Services;
using KidsMath.Contracts.Exercise;
using KidsMath.Contracts.Localization;
using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Api.Mapping;

public static class ExerciseSessionMapper
{
    public static ExerciseSessionResponse ToResponse(
        ExerciseSession session,
        string lang,
        int maxAttemptsPerQuestion,
        bool includeAnswers = false) =>
        new(
            session.Id,
            session.StudentProfileId,
            session.StartedAtUtc,
            session.FinishedAtUtc,
            session.Grade,
            session.TaskType,
            session.DifficultyLevel,
            session.TotalQuestions,
            session.CorrectAnswers,
            session.WrongAnswers,
            session.Status,
            (TemplateTheme)session.TemplateThemeId,
            maxAttemptsPerQuestion,
            session.Attempts
                .OrderBy(a => a.QuestionOrder)
                .Select(a => ToAttemptResponse(a, lang, maxAttemptsPerQuestion, includeAnswers))
                .ToList());

    public static ExerciseSessionListItemResponse ToListItem(ExerciseSession session) =>
        new(
            session.Id,
            session.StartedAtUtc,
            session.FinishedAtUtc,
            session.TaskType,
            session.CorrectAnswers,
            session.WrongAnswers,
            session.TotalQuestions,
            session.Status,
            (TemplateTheme)session.TemplateThemeId);

    public static SubmitAnswerResponse ToSubmitAnswerResponse(SubmitAnswerResult result) =>
        new(
            result.Attempt.Id,
            result.SubmissionIsCorrect,
            result.QuestionResolved,
            result.AttemptsUsed,
            result.MaxAttempts,
            result.Attempt.StudentAnswer,
            result.QuestionResolved ? result.Attempt.CorrectAnswer : null,
            result.Attempt.IsCorrect);

    private static ExerciseAttemptResponse ToAttemptResponse(
        ExerciseAttempt attempt,
        string lang,
        int maxAttemptsPerQuestion,
        bool includeAnswers)
    {
        var isResolved = attempt.IsCorrect != null;
        var submissions = attempt.AnswerSubmissions
            .OrderBy(s => s.AttemptNumber)
            .Select(s => new AnswerSubmissionResponse(
                s.AttemptNumber,
                s.Answer,
                s.IsCorrect,
                s.SubmittedAtUtc))
            .ToList();

        return new ExerciseAttemptResponse(
            attempt.Id,
            attempt.QuestionOrder,
            new LocalizedText(attempt.QuestionTextCs, attempt.QuestionTextEn).For(lang),
            attempt.QuestionTextCs,
            attempt.QuestionTextEn,
            (TemplateTheme)attempt.TemplateThemeId,
            attempt.GeneratedQuestionJson,
            includeAnswers && isResolved ? attempt.CorrectAnswer : null,
            attempt.StudentAnswer,
            attempt.IsCorrect,
            attempt.AnsweredAtUtc,
            isResolved,
            submissions.Count,
            maxAttemptsPerQuestion,
            submissions);
    }
}
