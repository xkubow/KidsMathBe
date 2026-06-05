using KidsMath.Domain.Enums;

namespace KidsMath.Contracts.MathTasks;

public record MathTaskDefinitionAdminDto(
    Guid Id,
    int Grade,
    TaskType TaskType,
    int DifficultyLevel,
    string DisplayNameCs,
    string DisplayNameEn,
    string? DescriptionCs,
    string? DescriptionEn,
    string ConfigJson,
    bool IsActive,
    DateTime CreatedAtUtc);