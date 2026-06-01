using KidsMath.Api.Extensions;
using KidsMath.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsMath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/students/{studentId:guid}")]
public class ProgressController(ProgressService progressService, StudentSummaryService summaryService, StudentService studentService) : ControllerBase
{
    [HttpGet("progress")]
    public async Task<ActionResult<object>> GetProgress(Guid studentId, CancellationToken ct)
    {
        if (!await CanAccessAsync(studentId, ct)) return Forbid();
        var progress = await progressService.GetProgressAsync(studentId, ct);
        return Ok(progress.Select(p => new
        {
            p.Grade,
            taskType = p.TaskType.ToString(),
            p.DifficultyLevel,
            p.TotalAttempts,
            p.CorrectAttempts,
            p.WrongAttempts,
            p.BestScore,
            p.CurrentStreak,
            p.LastPracticedAtUtc
        }));
    }

    [HttpGet("summary")]
    public async Task<ActionResult<object>> GetSummary(Guid studentId, [FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        if (!await CanAccessAsync(studentId, ct)) return Forbid();
        var summary = await summaryService.GetSummaryAsync(studentId, ct);
        return Ok(new
        {
            summary.StudentId,
            summary.Name,
            summary.Grade,
            summary.TotalAnswered,
            summary.TotalCorrect,
            progress = summary.Progress.Select(p => new
            {
                p.Grade,
                taskType = p.TaskType.ToString(),
                p.DifficultyLevel,
                p.TotalAttempts,
                p.CorrectAttempts,
                p.BestScore,
                p.CurrentStreak
            }),
            achievements = summary.Achievements.Select(a => new
            {
                a.UnlockedAtUtc,
                code = a.Achievement.Code,
                displayName = lang.StartsWith("en") ? a.Achievement.DisplayNameEn : a.Achievement.DisplayNameCs,
                description = lang.StartsWith("en") ? a.Achievement.DescriptionEn : a.Achievement.DescriptionCs
            }),
            recentSessions = summary.RecentSessions
        });
    }

    private async Task<bool> CanAccessAsync(Guid studentId, CancellationToken ct)
    {
        if (User.IsStudentToken()) return User.GetStudentId() == studentId;
        return await studentService.GetForParentAsync(User.GetParentUserId(), studentId, ct) is not null;
    }
}
