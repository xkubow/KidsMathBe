using System.Security.Claims;

namespace KidsMath.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetParentUserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public static Guid? GetStudentId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("student_id");
        return value is null ? null : Guid.Parse(value);
    }

    public static bool IsStudentToken(this ClaimsPrincipal user) =>
        user.FindFirstValue("token_type") == "student";
}
