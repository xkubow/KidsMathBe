namespace KidsMath.Application.Exercise;

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
}
