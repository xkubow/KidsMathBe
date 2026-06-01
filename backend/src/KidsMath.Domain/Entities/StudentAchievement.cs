namespace KidsMath.Domain.Entities;

public class StudentAchievement
{
    public Guid Id { get; set; }
    public Guid StudentProfileId { get; set; }
    public Guid AchievementId { get; set; }
    public DateTime UnlockedAtUtc { get; set; }

    public StudentProfile StudentProfile { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}
