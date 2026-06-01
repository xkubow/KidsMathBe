using KidsMath.Application.Abstractions;
using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KidsMath.Persistence;

public class KidsMathDbContext(DbContextOptions<KidsMathDbContext> options) : DbContext(options), IKidsMathDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<MathTaskDefinition> MathTaskDefinitions => Set<MathTaskDefinition>();
    public DbSet<ExerciseSession> ExerciseSessions => Set<ExerciseSession>();
    public DbSet<ExerciseAttempt> ExerciseAttempts => Set<ExerciseAttempt>();
    public DbSet<AnswerSubmission> AnswerSubmissions => Set<AnswerSubmission>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<StudentAchievement> StudentAchievements => Set<StudentAchievement>();
    public DbSet<StudentTaskProgress> StudentTaskProgress => Set<StudentTaskProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KidsMathDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
