using KidsMath.Api.Extensions;
using KidsMath.Api.Mapping;
using KidsMath.Application.Abstractions;
using KidsMath.Application.Services;
using KidsMath.Contracts.Achievements;
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
    public async Task<ActionResult<IReadOnlyList<AchievementDefinitionResponse>>> ListAll([FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        var items = await db.Achievements.AsNoTracking().Where(a => a.IsActive).ToListAsync(ct);
        return Ok(items.Select(a => AchievementMapper.ToDefinitionResponse(a, lang)).ToList());
    }

    [HttpGet("students/{studentId:guid}/achievements")]
    public async Task<ActionResult<IReadOnlyList<StudentAchievementResponse>>> ListForStudent(Guid studentId, [FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        if (User.IsStudentToken() && User.GetStudentId() != studentId) return Forbid();
        var items = await achievementService.GetStudentAchievementsAsync(studentId, ct);
        return Ok(items.Select(a => AchievementMapper.ToStudentResponse(a, lang)).ToList());
    }
}
