namespace KidsMath.Domain.Entities;

public class Achievement
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayNameCs { get; set; } = string.Empty;
    public string DisplayNameEn { get; set; } = string.Empty;
    public string DescriptionCs { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string ConditionType { get; set; } = string.Empty;
    public string ConditionJson { get; set; } = "{}";
    public bool IsActive { get; set; } = true;

    public ICollection<StudentAchievement> StudentAchievements { get; set; } = new List<StudentAchievement>();
}
