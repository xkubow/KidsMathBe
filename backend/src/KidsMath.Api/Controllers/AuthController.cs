using System.Security.Claims;
using KidsMath.Application.Services;
using KidsMath.Contracts.Auth;
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
        return Ok(new AuthResponse(token, user.Id, user.Email, user.DisplayName));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request.Email, request.Password, ct);
        if (result is null) return Unauthorized();
        var (user, token) = result.Value;
        return Ok(new AuthResponse(token, user.Id, user.Email, user.DisplayName));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<object>> Me(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await authService.GetUserAsync(userId, ct);
        if (user is null) return NotFound();
        return Ok(new { user.Id, user.Email, user.DisplayName, tokenType = User.FindFirstValue("token_type") ?? "parent" });
    }
}
