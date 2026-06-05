using KidsMath.Domain.Enums;

namespace KidsMath.Contracts.Exercise;

public record ExerciseSessionListItemResponse(
    Guid Id,
    DateTime StartedAtUtc,
    DateTime? FinishedAtUtc,
    TaskType TaskType,
    int CorrectAnswers,
    int WrongAnswers,
    int TotalQuestions,
    SessionStatus Status,
    TemplateTheme Theme);
