namespace KidsMath.Application.Options;

public class ExerciseOptions
{
    public const string SectionName = "Exercise";

    public int DefaultQuestionCount { get; set; } = 10;

    public int MaxAttemptsPerQuestion { get; set; } = 10;
}
