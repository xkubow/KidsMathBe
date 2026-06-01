using KidsMath.Api.Extensions;
using KidsMath.Application.Abstractions;
using KidsMath.Application.Services;
using KidsMath.Contracts.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KidsMath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class AchievementsController(IKidsMathDbContext db, AchievementService achievementService) : ControllerBase
{
    [HttpGet("achievements")]
    public async Task<ActionResult<IEnumerable<object>>> ListAll([FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        var items = await db.Achievements.AsNoTracking().Where(a => a.IsActive).ToListAsync(ct);
        return Ok(items.Select(a => new
        {
            a.Id,
            a.Code,
            displayName = new LocalizedText(a.DisplayNameCs, a.DisplayNameEn).For(lang),
            description = new LocalizedText(a.DescriptionCs, a.DescriptionEn).For(lang)
        }));
    }

    [HttpGet("students/{studentId:guid}/achievements")]
    public async Task<ActionResult<IEnumerable<object>>> ListForStudent(Guid studentId, [FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        if (User.IsStudentToken() && User.GetStudentId() != studentId) return Forbid();
        var items = await achievementService.GetStudentAchievementsAsync(studentId, ct);
        return Ok(items.Select(a => new
        {
            a.UnlockedAtUtc,
            code = a.Achievement.Code,
            displayName = new LocalizedText(a.Achievement.DisplayNameCs, a.Achievement.DisplayNameEn).For(lang),
            description = new LocalizedText(a.Achievement.DescriptionCs, a.Achievement.DescriptionEn).For(lang)
        }));
    }
}
