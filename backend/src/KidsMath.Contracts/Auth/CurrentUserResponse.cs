namespace KidsMath.Contracts.Auth;

public record CurrentUserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    bool IsAdmin,
    string TokenType,
    Guid? StudentId);
