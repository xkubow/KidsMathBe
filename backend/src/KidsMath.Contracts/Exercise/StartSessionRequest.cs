using KidsMath.Domain.Enums;

namespace KidsMath.Contracts.Exercise;

public record StartSessionRequest(
    Guid StudentProfileId,
    Guid TaskDefinitionId,
    int? QuestionCount,
    TemplateTheme? Theme = null);
