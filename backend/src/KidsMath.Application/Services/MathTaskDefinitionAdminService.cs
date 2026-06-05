using KidsMath.Application.Abstractions;
using KidsMath.Contracts.MathTasks;
using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KidsMath.Application.Services;

public class MathTaskDefinitionAdminService(IKidsMathDbContext db)
{
    public async Task<IReadOnlyList<MathTaskDefinitionAdminDto>> ListAllAsync(CancellationToken ct = default)
    {
        var items = await db.MathTaskDefinitions.AsNoTracking()
            .OrderBy(d => d.Grade)
            .ThenBy(d => d.TaskType)
            .ThenBy(d => d.DifficultyLevel)
            .ToListAsync(ct);
        return items.Select(ToDto).ToList();
    }

    public async Task<MathTaskDefinitionAdminDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.MathTaskDefinitions.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<MathTaskDefinitionAdminDto?> CreateAsync(CreateMathTaskDefinitionRequest request, CancellationToken ct = default)
    {
        if (!TryParseTaskType(request.TaskType, out var taskType)) return null;

        var entity = new MathTaskDefinition
        {
            Id = Guid.NewGuid(),
            Grade = request.Grade,
            TaskType = taskType,
            DifficultyLevel = request.DifficultyLevel,
            DisplayNameCs = request.DisplayNameCs.Trim(),
            DisplayNameEn = request.DisplayNameEn.Trim(),
            DescriptionCs = request.DescriptionCs?.Trim(),
            DescriptionEn = request.DescriptionEn?.Trim(),
            ConfigJson = string.IsNullOrWhiteSpace(request.ConfigJson) ? "{}" : request.ConfigJson.Trim(),
            IsActive = request.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.MathTaskDefinitions.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<MathTaskDefinitionAdminDto?> UpdateAsync(Guid id, UpdateMathTaskDefinitionRequest request, CancellationToken ct = default)
    {
        if (!TryParseTaskType(request.TaskType, out var taskType)) return null;

        var entity = await db.MathTaskDefinitions.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (entity is null) return null;

        entity.Grade = request.Grade;
        entity.TaskType = taskType;
        entity.DifficultyLevel = request.DifficultyLevel;
        entity.DisplayNameCs = request.DisplayNameCs.Trim();
        entity.DisplayNameEn = request.DisplayNameEn.Trim();
        entity.DescriptionCs = request.DescriptionCs?.Trim();
        entity.DescriptionEn = request.DescriptionEn?.Trim();
        entity.ConfigJson = string.IsNullOrWhiteSpace(request.ConfigJson) ? "{}" : request.ConfigJson.Trim();
        entity.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.MathTaskDefinitions.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (entity is null) return false;
        entity.IsActive = false;
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static bool TryParseTaskType(string value, out TaskType taskType) =>
        Enum.TryParse(value, true, out taskType);

    private static MathTaskDefinitionAdminDto ToDto(MathTaskDefinition d) => new(
        d.Id,
        d.Grade,
        d.TaskType,
        d.DifficultyLevel,
        d.DisplayNameCs,
        d.DisplayNameEn,
        d.DescriptionCs,
        d.DescriptionEn,
        d.ConfigJson,
        d.IsActive,
        d.CreatedAtUtc);
}
