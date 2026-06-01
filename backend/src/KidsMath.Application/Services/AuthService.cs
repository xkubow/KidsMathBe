using KidsMath.Application.Abstractions;
using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KidsMath.Application.Services;

public class AuthService(IKidsMathDbContext db, JwtTokenService jwt)
{
    public async Task<(User User, string Token)?> RegisterAsync(string email, string password, string displayName, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == normalized, ct))
        {
            return null;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalized,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            DisplayName = displayName.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return (user, jwt.CreateParentToken(user.Id, user.Email, user.DisplayName));
    }

    public async Task<(User User, string Token)?> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == normalized, ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        return (user, jwt.CreateParentToken(user.Id, user.Email, user.DisplayName));
    }

    public async Task<User?> GetUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
}
