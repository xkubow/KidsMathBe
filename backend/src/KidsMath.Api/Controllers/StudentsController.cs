using KidsMath.Api.Extensions;
using KidsMath.Application.Services;
using KidsMath.Contracts.Auth;
using KidsMath.Contracts.Students;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsMath.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/students")]
public class StudentsController(StudentService studentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentResponse>>> List(CancellationToken ct)
    {
        var students = await studentService.ListForParentAsync(User.GetParentUserId(), ct);
        return Ok(students.Select(Map));
    }

    [HttpPost]
    public async Task<ActionResult<StudentResponse>> Create(CreateStudentRequest request, CancellationToken ct)
    {
        var student = await studentService.CreateAsync(
            User.GetParentUserId(), request.Name, request.Grade, request.Pin, request.AvatarKey, ct);
        return CreatedAtAction(nameof(Get), new { studentId = student.Id }, Map(student));
    }

    [HttpGet("{studentId:guid}")]
    public async Task<ActionResult<StudentResponse>> Get(Guid studentId, CancellationToken ct)
    {
        var student = await studentService.GetForParentAsync(User.GetParentUserId(), studentId, ct);
        return student is null ? NotFound() : Ok(Map(student));
    }

    [HttpPut("{studentId:guid}")]
    public async Task<ActionResult<StudentResponse>> Update(Guid studentId, UpdateStudentRequest request, CancellationToken ct)
    {
        var student = await studentService.UpdateAsync(
            User.GetParentUserId(), studentId, request.Name, request.Grade, request.Pin, request.AvatarKey, ct);
        return student is null ? NotFound() : Ok(Map(student));
    }

    [HttpDelete("{studentId:guid}")]
    public async Task<IActionResult> Delete(Guid studentId, CancellationToken ct)
    {
        var ok = await studentService.DeleteAsync(User.GetParentUserId(), studentId, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{studentId:guid}/verify-pin")]
    public async Task<ActionResult<AuthResponse>> VerifyPin(Guid studentId, VerifyPinRequest request, CancellationToken ct)
    {
        var token = await studentService.VerifyPinAsync(User.GetParentUserId(), studentId, request.Pin, ct);
        if (token is null) return BadRequest("Invalid PIN.");
        var student = await studentService.GetForParentAsync(User.GetParentUserId(), studentId, ct);
        return Ok(new AuthResponse(token, User.GetParentUserId(), "", student!.Name));
    }

    [HttpPost("{studentId:guid}/reset-pin")]
    public async Task<IActionResult> ResetPin(Guid studentId, ResetPinRequest request, CancellationToken ct)
    {
        var ok = await studentService.ResetPinAsync(User.GetParentUserId(), studentId, request.Pin, ct);
        return ok ? NoContent() : BadRequest("Invalid PIN or student not found.");
    }

    private static StudentResponse Map(Domain.Entities.StudentProfile s) =>
        new(s.Id, s.Name, s.Grade, s.AvatarKey, s.CreatedAtUtc);
}
