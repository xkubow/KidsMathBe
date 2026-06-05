using KidsMath.Api.Extensions;
using KidsMath.Api.Mapping;
using KidsMath.Application.Services;
using KidsMath.Contracts.Progress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsMath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/students/{studentId:guid}")]
public class ProgressController(ProgressService progressService, StudentSummaryService summaryService, StudentService studentService) : ControllerBase
{
    [HttpGet("progress")]
    public async Task<ActionResult<IReadOnlyList<StudentTaskProgressResponse>>> GetProgress(Guid studentId, CancellationToken ct)
    {
        if (!await CanAccessAsync(studentId, ct)) return Forbid();
        var progress = await progressService.GetProgressAsync(studentId, ct);
        return Ok(progress.Select(ProgressMapper.ToResponse).ToList());
    }

    [HttpGet("summary")]
    public async Task<ActionResult<StudentSummaryResponse>> GetSummary(Guid studentId, [FromQuery] string lang = "cs", CancellationToken ct = default)
    {
        if (!await CanAccessAsync(studentId, ct)) return Forbid();
        var summary = await summaryService.GetSummaryAsync(studentId, ct);
        return Ok(ProgressMapper.ToSummaryResponse(summary, lang));
    }

    private async Task<bool> CanAccessAsync(Guid studentId, CancellationToken ct)
    {
        if (User.IsStudentToken()) return User.GetStudentId() == studentId;
        return await studentService.GetForParentAsync(User.GetParentUserId(), studentId, ct) is not null;
    }
}
