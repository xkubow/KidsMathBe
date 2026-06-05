namespace KidsMath.Application.Exercise;

public sealed class StaticExerciseConfig
{
    public string QuestionTextCs { get; set; } = string.Empty;
    public string QuestionTextEn { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public System.Text.Json.JsonElement? QuestionData { get; set; }
}

public sealed class TaskConfig
{
    public int MinNumber { get; set; }
    public int MaxNumber { get; set; } = 20;
    public int TermsCount { get; set; } = 2;
    public bool AllowNegativeResult { get; set; }
    public bool AllowCarry { get; set; } = true;
    public bool AllowBorrow { get; set; } = true;
    public int[]? Multipliers { get; set; }
    public int SequenceStep { get; set; } = 2;
    public string[]? Fractions { get; set; }
    public string[]? Shapes { get; set; }

    // If set, a random static question can be injected into the generated stream.
    // 0 = never, 100 = always (when static exercises exist).
    public int StaticExerciseChancePercent { get; set; } = 0;
    public StaticExerciseConfig[]? StaticExercises { get; set; }
}
