using KidsMath.Application.Exercise;
using KidsMath.Application.Exercise.Generators;
using KidsMath.Application.Options;
using KidsMath.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KidsMath.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddKidsMathApplication(this IServiceCollection services)
    {
        services.Configure<AchievementOptions>(options => { });
        services.Configure<ExerciseOptions>(options => { });

        services.AddSingleton<IRandomNumberSource, RandomNumberSource>();
        services.AddSingleton<IExerciseGenerator, AdditionExerciseGenerator>();
        services.AddSingleton<IExerciseGenerator, SubtractionExerciseGenerator>();
        services.AddSingleton<IExerciseGenerator, ComparisonExerciseGenerator>();
        services.AddSingleton<IExerciseGenerator, MissingNumberExerciseGenerator>();
        services.AddSingleton<IExerciseGenerator, MultiplicationExerciseGenerator>();
        services.AddSingleton<IExerciseGenerator, DivisionExerciseGenerator>();
        services.AddSingleton<IExerciseGenerator, NumberSequenceExerciseGenerator>();
        services.AddSingleton<IExerciseGenerator, EvenOddExerciseGenerator>();
        services.AddSingleton<IExerciseGenerator, FractionsBasicExerciseGenerator>();
        services.AddSingleton<IExerciseGenerator, GeometryBasicExerciseGenerator>();
        services.AddSingleton<IExerciseGenerator, WordProblemExerciseGenerator>();
        services.AddSingleton<ExerciseGeneratorFactory>();

        services.AddScoped<AuthService>();
        services.AddScoped<StudentService>();
        services.AddScoped<ExerciseSessionService>();
        services.AddScoped<ProgressService>();
        services.AddScoped<AchievementService>();
        services.AddScoped<StudentSummaryService>();
        services.AddScoped<JwtTokenService>();

        return services;
    }
}
