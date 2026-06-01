using KidsMath.Application.Abstractions;
using KidsMath.Contracts.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidsMath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/math")]
public class MathController(IKidsMathDbContext db) : ControllerBase
{
    [HttpGet("task-definitions")]
    public async Task<ActionResult<IEnumerable<object>>> GetDefinitions(
        [FromQuery] int? grade,
        [FromQuery] string? taskType,
        [FromQuery] string lang = "cs",
        CancellationToken ct = default)
    {
        var query = db.MathTaskDefinitions.AsNoTracking().Where(d => d.IsActive);
        if (grade.HasValue) query = query.Where(d => d.Grade == grade);
        if (!string.IsNullOrWhiteSpace(taskType) && Enum.TryParse<Domain.Enums.TaskType>(taskType, true, out var tt))
        {
            query = query.Where(d => d.TaskType == tt);
        }

        var items = await query.OrderBy(d => d.Grade).ThenBy(d => d.TaskType).ThenBy(d => d.DifficultyLevel).ToListAsync(ct);
        return Ok(items.Select(d => new
        {
            d.Id,
            d.Grade,
            taskType = d.TaskType.ToString(),
            d.DifficultyLevel,
            displayName = new LocalizedText(d.DisplayNameCs, d.DisplayNameEn).For(lang),
            description = d.DescriptionCs is null ? null : new LocalizedText(d.DescriptionCs, d.DescriptionEn ?? d.DescriptionCs).For(lang),
            d.ConfigJson
        }));
    }
}
