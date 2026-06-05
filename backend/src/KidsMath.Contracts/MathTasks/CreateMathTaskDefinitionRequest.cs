namespace KidsMath.Contracts.MathTasks;

public record CreateMathTaskDefinitionRequest(
    int Grade,
    string TaskType,
    int DifficultyLevel,
    string DisplayNameCs,
    string DisplayNameEn,
    string? DescriptionCs,
    string? DescriptionEn,
    string ConfigJson,
    bool IsActive = true);