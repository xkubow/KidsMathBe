using KidsMath.Contracts.Achievements;
using KidsMath.Contracts.Localization;
using KidsMath.Domain.Entities;

namespace KidsMath.Api.Mapping;

public static class AchievementMapper
{
    public static AchievementDefinitionResponse ToDefinitionResponse(Achievement achievement, string lang) =>
        new(
            achievement.Id,
            achievement.Code,
            new LocalizedText(achievement.DisplayNameCs, achievement.DisplayNameEn).For(lang),
            new LocalizedText(achievement.DescriptionCs, achievement.DescriptionEn).For(lang));

    public static StudentAchievementResponse ToStudentResponse(StudentAchievement achievement, string lang) =>
        new(
            achievement.UnlockedAtUtc,
            achievement.Achievement.Code,
            new LocalizedText(achievement.Achievement.DisplayNameCs, achievement.Achievement.DisplayNameEn).For(lang),
            new LocalizedText(achievement.Achievement.DescriptionCs, achievement.Achievement.DescriptionEn).For(lang));
}
