using KidsMath.Domain.Enums;

namespace KidsMath.Domain.Entities;

public class ExerciseSession
{
    public Guid Id { get; set; }
    public Guid StudentProfileId { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? FinishedAtUtc { get; set; }
    public int Grade { get; set; }
    public TaskType TaskType { get; set; }
    public int DifficultyLevel { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public SessionStatus Status { get; set; }

    public StudentProfile StudentProfile { get; set; } = null!;
    public ICollection<ExerciseAttempt> Attempts { get; set; } = new List<ExerciseAttempt>();
}
