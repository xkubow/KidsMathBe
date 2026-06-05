using KidsMath.Domain.Enums;

namespace KidsMath.Contracts.Progress;

public record RecentSessionResponse(
    Guid Id,
    DateTime StartedAtUtc,
    DateTime? FinishedAtUtc,
    TaskType TaskType,
    int CorrectAnswers,
    int WrongAnswers,
    int TotalQuestions,
    SessionStatus Status);
