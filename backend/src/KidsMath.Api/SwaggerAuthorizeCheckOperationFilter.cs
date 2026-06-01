using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KidsMath.Api;

public sealed class SwaggerAuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var methodInfo = context.MethodInfo;
        if (methodInfo is null)
            return;

        // Respect [AllowAnonymous] at method or controller level
        if (HasAttribute<AllowAnonymousAttribute>(methodInfo) ||
            HasAttribute<AllowAnonymousAttribute>(methodInfo.DeclaringType))
        {
            return;
        }

        var requiresAuth =
            HasAttribute<AuthorizeAttribute>(methodInfo) ||
            HasAttribute<AuthorizeAttribute>(methodInfo.DeclaringType);

        if (!requiresAuth)
            return;

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }

    private static bool HasAttribute<T>(MemberInfo? memberInfo) where T : Attribute
        => memberInfo?.GetCustomAttributes(typeof(T), inherit: true).Any() == true;
}

