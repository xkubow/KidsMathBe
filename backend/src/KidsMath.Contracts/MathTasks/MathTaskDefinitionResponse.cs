using KidsMath.Domain.Enums;

namespace KidsMath.Contracts.MathTasks;

public record MathTaskDefinitionResponse(
    Guid Id,
    int Grade,
    TaskType TaskType,
    int DifficultyLevel,
    string DisplayName,
    string? Description,
    string ConfigJson);
