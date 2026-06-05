namespace KidsMath.Contracts.Achievements;

public record StudentAchievementResponse(
    DateTime UnlockedAtUtc,
    string Code,
    string DisplayName,
    string Description);
