namespace KidsMath.Domain.Entities;

public class AnswerSubmission
{
    public Guid Id { get; set; }
    public Guid ExerciseAttemptId { get; set; }
    public int AttemptNumber { get; set; }
    public string Answer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public DateTime SubmittedAtUtc { get; set; }

    public ExerciseAttempt ExerciseAttempt { get; set; } = null!;
}
