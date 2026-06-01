using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KidsMath.Application.Abstractions;

public interface IKidsMathDbContext
{
    DbSet<User> Users { get; }
    DbSet<StudentProfile> StudentProfiles { get; }
    DbSet<MathTaskDefinition> MathTaskDefinitions { get; }
    DbSet<ExerciseSession> ExerciseSessions { get; }
    DbSet<ExerciseAttempt> ExerciseAttempts { get; }
    DbSet<AnswerSubmission> AnswerSubmissions { get; }
    DbSet<Achievement> Achievements { get; }
    DbSet<StudentAchievement> StudentAchievements { get; }
    DbSet<StudentTaskProgress> StudentTaskProgress { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
