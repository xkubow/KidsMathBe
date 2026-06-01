namespace KidsMath.Contracts.Students;

public record CreateStudentRequest(string Name, int Grade, string Pin, string? AvatarKey);
