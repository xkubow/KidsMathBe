using KidsMath.Domain.Enums;

namespace KidsMath.Domain.Entities;

public class MathTaskDefinition
{
    public Guid Id { get; set; }
    public int Grade { get; set; }
    public TaskType TaskType { get; set; }
    public int DifficultyLevel { get; set; }
    public string DisplayNameCs { get; set; } = string.Empty;
    public string DisplayNameEn { get; set; } = string.Empty;
    public string? DescriptionCs { get; set; }
    public string? DescriptionEn { get; set; }
    public string ConfigJson { get; set; } = "{}";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
}
