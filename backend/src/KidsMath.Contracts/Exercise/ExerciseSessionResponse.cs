using KidsMath.Domain.Enums;

namespace KidsMath.Contracts.Exercise;

public record ExerciseSessionResponse(
    Guid Id,
    Guid StudentProfileId,
    DateTime StartedAtUtc,
    DateTime? FinishedAtUtc,
    int Grade,
    TaskType TaskType,
    int DifficultyLevel,
    int TotalQuestions,
    int CorrectAnswers,
    int WrongAnswers,
    SessionStatus Status,
    TemplateTheme Theme,
    int MaxAttemptsPerQuestion,
    IReadOnlyList<ExerciseAttemptResponse> Attempts);
