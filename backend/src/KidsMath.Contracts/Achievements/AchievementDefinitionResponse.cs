namespace KidsMath.Contracts.Achievements;

public record AchievementDefinitionResponse(
    Guid Id,
    string Code,
    string DisplayName,
    string Description);
