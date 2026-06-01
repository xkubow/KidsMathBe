namespace KidsMath.Contracts.Exercise;

public record StartSessionRequest(Guid StudentProfileId, Guid TaskDefinitionId, int? QuestionCount);
