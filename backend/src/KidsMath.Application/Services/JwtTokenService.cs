using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace KidsMath.Application.Services;

public class JwtTokenService(IConfiguration configuration)
{
    public string CreateParentToken(Guid userId, string email, string displayName)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, displayName),
            new("token_type", "parent")
        };
        return CreateToken(claims);
    }

    public string CreateStudentToken(Guid parentUserId, Guid studentId, string studentName)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, parentUserId.ToString()),
            new("student_id", studentId.ToString()),
            new("student_name", studentName),
            new("token_type", "student")
        };
        return CreateToken(claims);
    }

    private string CreateToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSigningKey()));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiryHours = int.TryParse(configuration["Jwt:ExpiryHours"], out var h) ? h : 12;
        var expiry = DateTime.UtcNow.AddHours(expiryHours);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GetSigningKey() =>
        configuration["Jwt:SigningKey"]
        ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");
}
