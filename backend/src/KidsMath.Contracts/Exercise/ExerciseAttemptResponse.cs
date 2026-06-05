using KidsMath.Domain.Enums;

namespace KidsMath.Contracts.Exercise;

public record ExerciseAttemptResponse(
    Guid Id,
    int QuestionOrder,
    string QuestionText,
    string QuestionTextCs,
    string QuestionTextEn,
    TemplateTheme Theme,
    string GeneratedQuestionJson,
    string? CorrectAnswer,
    string? StudentAnswer,
    bool? IsCorrect,
    DateTime? AnsweredAtUtc,
    bool IsResolved,
    int AttemptsUsed,
    int MaxAttempts,
    IReadOnlyList<AnswerSubmissionResponse> Submissions);
