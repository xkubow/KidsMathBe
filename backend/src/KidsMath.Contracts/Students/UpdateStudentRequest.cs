namespace KidsMath.Contracts.Students;

public record UpdateStudentRequest(string Name, int Grade, string? Pin, string? AvatarKey);
