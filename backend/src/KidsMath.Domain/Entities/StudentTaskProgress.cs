using KidsMath.Domain.Enums;

namespace KidsMath.Domain.Entities;

public class StudentTaskProgress
{
    public Guid Id { get; set; }
    public Guid StudentProfileId { get; set; }
    public int Grade { get; set; }
    public TaskType TaskType { get; set; }
    public int DifficultyLevel { get; set; }
    public int TotalAttempts { get; set; }
    public int CorrectAttempts { get; set; }
    public int WrongAttempts { get; set; }
    public int BestScore { get; set; }
    public int CurrentStreak { get; set; }
    public DateTime? LastPracticedAtUtc { get; set; }

    public StudentProfile StudentProfile { get; set; } = null!;
}
