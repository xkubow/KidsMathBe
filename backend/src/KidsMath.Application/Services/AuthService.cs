using KidsMath.Application.Abstractions;
using KidsMath.Application.Options;
using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KidsMath.Application.Services;

public class AuthService(IKidsMathDbContext db, JwtTokenService jwt, IOptions<AdminOptions> adminOptions)
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
            IsAdmin = IsConfiguredAdminEmail(normalized),
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return (user, jwt.CreateParentToken(user.Id, user.Email, user.DisplayName, user.IsAdmin));
    }

    public async Task<(User User, string Token)?> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == normalized, ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        await SyncAdminFlagAsync(user, ct);
        return (user, jwt.CreateParentToken(user.Id, user.Email, user.DisplayName, user.IsAdmin));
    }

    public async Task<User?> GetUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);

    public async Task<(User User, string Token)?> SwitchToParentAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null) return null;

        return (user, jwt.CreateParentToken(user.Id, user.Email, user.DisplayName, user.IsAdmin));
    }

    public async Task<(User User, string Token)?> SwitchToAdminAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null || !user.IsAdmin) return null;

        return (user, jwt.CreateAdminToken(user.Id, user.Email, user.DisplayName));
    }

    private bool IsConfiguredAdminEmail(string normalizedEmail) =>
        adminOptions.Value.Emails
            .Select(e => e.Trim().ToLowerInvariant())
            .Contains(normalizedEmail);

    private async Task SyncAdminFlagAsync(User user, CancellationToken ct)
    {
        var shouldBeAdmin = IsConfiguredAdminEmail(user.Email);
        if (user.IsAdmin == shouldBeAdmin) return;

        user.IsAdmin = shouldBeAdmin;
        await db.SaveChangesAsync(ct);
    }
}
