namespace KidsMath.Contracts.Students;

public record StudentResponse(Guid Id, string Name, int Grade, string? AvatarKey, DateTime CreatedAtUtc);
