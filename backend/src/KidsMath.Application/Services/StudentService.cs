using KidsMath.Application.Abstractions;
using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KidsMath.Application.Services;

public class StudentService(IKidsMathDbContext db, JwtTokenService jwt)
{
    public async Task<IReadOnlyList<StudentProfile>> ListForParentAsync(Guid parentId, CancellationToken ct = default) =>
        await db.StudentProfiles.AsNoTracking()
            .Where(s => s.ParentUserId == parentId)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);

    public async Task<StudentProfile?> GetForParentAsync(Guid parentId, Guid studentId, CancellationToken ct = default) =>
        await db.StudentProfiles.FirstOrDefaultAsync(s => s.ParentUserId == parentId && s.Id == studentId, ct);

    public async Task<StudentProfile> CreateAsync(Guid parentId, string name, int grade, string pin, string? avatarKey, CancellationToken ct = default)
    {
        if (grade is < 1 or > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(grade), "Grade must be 1–3.");
        }

        var student = new StudentProfile
        {
            Id = Guid.NewGuid(),
            ParentUserId = parentId,
            Name = name.Trim(),
            Grade = grade,
            AvatarKey = avatarKey,
            PinHash = BCrypt.Net.BCrypt.HashPassword(pin),
            CreatedAtUtc = DateTime.UtcNow
        };
        db.StudentProfiles.Add(student);
        await db.SaveChangesAsync(ct);
        return student;
    }

    public async Task<StudentProfile?> UpdateAsync(Guid parentId, Guid studentId, string name, int grade, string? pin, string? avatarKey, CancellationToken ct = default)
    {
        var student = await GetForParentAsync(parentId, studentId, ct);
        if (student is null) return null;

        student.Name = name.Trim();
        student.Grade = grade;
        student.AvatarKey = avatarKey;
        if (!string.IsNullOrWhiteSpace(pin))
        {
            student.PinHash = BCrypt.Net.BCrypt.HashPassword(pin);
        }

        await db.SaveChangesAsync(ct);
        return student;
    }

    public async Task<bool> DeleteAsync(Guid parentId, Guid studentId, CancellationToken ct = default)
    {
        var student = await GetForParentAsync(parentId, studentId, ct);
        if (student is null) return false;
        db.StudentProfiles.Remove(student);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<string?> VerifyPinAsync(Guid parentId, Guid studentId, string pin, CancellationToken ct = default)
    {
        var student = await GetForParentAsync(parentId, studentId, ct);
        if (student is null || !BCrypt.Net.BCrypt.Verify(pin, student.PinHash))
        {
            return null;
        }

        return jwt.CreateStudentToken(parentId, student.Id, student.Name);
    }

    public async Task<bool> ResetPinAsync(Guid parentId, Guid studentId, string pin, CancellationToken ct = default)
    {
        if (!IsValidPin(pin))
        {
            return false;
        }

        var student = await GetForParentAsync(parentId, studentId, ct);
        if (student is null) return false;

        student.PinHash = BCrypt.Net.BCrypt.HashPassword(pin);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static bool IsValidPin(string pin) =>
        pin.Length is >= 4 and <= 6 && pin.All(char.IsDigit);
}
