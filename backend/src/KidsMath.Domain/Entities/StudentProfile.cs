namespace KidsMath.Domain.Entities;

public class StudentProfile
{
    public Guid Id { get; set; }
    public Guid ParentUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string? AvatarKey { get; set; }
    public string PinHash { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    public User ParentUser { get; set; } = null!;
    public ICollection<ExerciseSession> ExerciseSessions { get; set; } = new List<ExerciseSession>();
    public ICollection<StudentAchievement> StudentAchievements { get; set; } = new List<StudentAchievement>();
    public ICollection<StudentTaskProgress> TaskProgress { get; set; } = new List<StudentTaskProgress>();
}
