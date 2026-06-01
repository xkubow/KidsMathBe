using KidsMath.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KidsMath.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddKidsMathPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("KidsMath")
            ?? "Host=localhost;Port=5432;Database=kids_math;Username=postgres;Password=postgres";

        var useInMemory = configuration.GetValue("Database:UseInMemory", false);
        services.AddDbContext<KidsMathDbContext>(options =>
        {
            if (useInMemory)
            {
                options.UseInMemoryDatabase(configuration["Database:InMemoryName"] ?? "kids_math_test");
            }
            else
            {
                options.UseNpgsql(connectionString);
            }
        });

        services.AddScoped<IKidsMathDbContext>(sp => sp.GetRequiredService<KidsMathDbContext>());

        return services;
    }
}
