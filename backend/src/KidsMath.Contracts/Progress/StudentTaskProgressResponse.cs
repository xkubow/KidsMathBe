using KidsMath.Domain.Enums;

namespace KidsMath.Contracts.Progress;

public record StudentTaskProgressResponse(
    int Grade,
    TaskType TaskType,
    int DifficultyLevel,
    int TotalAttempts,
    int CorrectAttempts,
    int WrongAttempts,
    int BestScore,
    int CurrentStreak,
    DateTime? LastPracticedAtUtc);
