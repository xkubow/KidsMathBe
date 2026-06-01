namespace KidsMath.Domain.Entities;

public class ExerciseAttempt
{
    public Guid Id { get; set; }
    public Guid ExerciseSessionId { get; set; }
    public Guid StudentProfileId { get; set; }
    public Guid MathTaskDefinitionId { get; set; }
    public int QuestionOrder { get; set; }
    public string GeneratedQuestionJson { get; set; } = "{}";
    public string QuestionTextCs { get; set; } = string.Empty;
    public string QuestionTextEn { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? StudentAnswer { get; set; }
    public bool? IsCorrect { get; set; }
    public DateTime? AnsweredAtUtc { get; set; }

    public ExerciseSession ExerciseSession { get; set; } = null!;
    public MathTaskDefinition MathTaskDefinition { get; set; } = null!;
    public ICollection<AnswerSubmission> AnswerSubmissions { get; set; } = new List<AnswerSubmission>();
}
