using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise;

public sealed class GeneratedExercise
{
    public required string QuestionTextCs { get; init; }
    public required string QuestionTextEn { get; init; }
    public required string CorrectAnswer { get; init; }
    public required object QuestionData { get; init; }
    public TemplateTheme Theme { get; init; } = TemplateTheme.Default;
}
