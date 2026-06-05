using KidsMath.Api.Extensions;
using KidsMath.Application.Services;
using KidsMath.Contracts.Auth;
using KidsMath.Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsMath.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request.Email, request.Password, request.DisplayName, ct);
        if (result is null) return Conflict("Email already registered.");
        var (user, token) = result.Value;
        return Ok(new AuthResponse(token, user.Id, user.Email, user.DisplayName, user.IsAdmin));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request.Email, request.Password, ct);
        if (result is null) return Unauthorized();
        var (user, token) = result.Value;
        return Ok(new AuthResponse(token, user.Id, user.Email, user.DisplayName, user.IsAdmin));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserResponse>> Me(CancellationToken ct)
    {
        var userId = User.GetParentUserId();
        var user = await authService.GetUserAsync(userId, ct);
        if (user is null) return NotFound();
        return Ok(new CurrentUserResponse(
            user.Id,
            user.Email,
            user.DisplayName,
            user.IsAdmin,
            User.IsStudentToken() ? "student" : User.IsAdminToken() ? "admin" : "parent",
            User.GetStudentId()));
    }

    [Authorize]
    [HttpPost("switch-to-parent")]
    public async Task<ActionResult<AuthResponse>> SwitchToParent(CancellationToken ct)
    {
        var userId = User.GetParentUserId();
        var result = await authService.SwitchToParentAsync(userId, ct);
        if (result is null) return NotFound();
        var (user, token) = result.Value;
        return Ok(new AuthResponse(token, user.Id, user.Email, user.DisplayName, user.IsAdmin));
    }

    [Authorize]
    [HttpPost("switch-to-admin")]
    public async Task<ActionResult<AuthResponse>> SwitchToAdmin(CancellationToken ct)
    {
        if (!User.IsAdminUser()) return Forbid();
        var userId = User.GetParentUserId();
        var result = await authService.SwitchToAdminAsync(userId, ct);
        if (result is null) return Forbid();
        var (user, token) = result.Value;
        return Ok(new AuthResponse(token, user.Id, user.Email, user.DisplayName, user.IsAdmin));
    }

    [Authorize]
    [HttpPost("logout")]
    public ActionResult<MessageResponse> Logout() =>
        Ok(new MessageResponse("Logged out successfully. Please remove the token from your client storage."));
}
