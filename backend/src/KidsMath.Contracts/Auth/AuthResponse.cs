namespace KidsMath.Contracts.Auth;

public record AuthResponse(string Token, Guid UserId, string Email, string DisplayName, bool IsAdmin = false);
