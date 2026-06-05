using KidsMath.Contracts.MathTasks;
using KidsMath.Contracts.Localization;
using KidsMath.Domain.Entities;

namespace KidsMath.Api.Mapping;

public static class MathTaskDefinitionMapper
{
    public static MathTaskDefinitionResponse ToPublicResponse(MathTaskDefinition definition, string lang) =>
        new(
            definition.Id,
            definition.Grade,
            definition.TaskType,
            definition.DifficultyLevel,
            new LocalizedText(definition.DisplayNameCs, definition.DisplayNameEn).For(lang),
            definition.DescriptionCs is null
                ? null
                : new LocalizedText(definition.DescriptionCs, definition.DescriptionEn ?? definition.DescriptionCs).For(lang),
            definition.ConfigJson);

    public static MathTaskDefinitionAdminDto ToAdminDto(MathTaskDefinition definition) =>
        new(
            definition.Id,
            definition.Grade,
            definition.TaskType,
            definition.DifficultyLevel,
            definition.DisplayNameCs,
            definition.DisplayNameEn,
            definition.DescriptionCs,
            definition.DescriptionEn,
            definition.ConfigJson,
            definition.IsActive,
            definition.CreatedAtUtc);
}
