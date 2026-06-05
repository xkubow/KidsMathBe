using KidsMath.Application.Abstractions;
using KidsMath.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KidsMath.Application.Services;

public class StudentSummaryService(
    IKidsMathDbContext db,
    ProgressService progressService,
    AchievementService achievementService)
{
    public async Task<StudentSummary> GetSummaryAsync(Guid studentId, CancellationToken ct = default)
    {
        var student = await db.StudentProfiles.AsNoTracking().FirstOrDefaultAsync(s => s.Id == studentId, ct)
                      ?? throw new KeyNotFoundException("Student not found.");

        var progress = await progressService.GetProgressAsync(studentId, ct);
        var achievements = await achievementService.GetStudentAchievementsAsync(studentId, ct);

        var recentSessions = await db.ExerciseSessions.AsNoTracking()
            .Where(s => s.StudentProfileId == studentId)
            .OrderByDescending(s => s.StartedAtUtc)
            .Take(5)
            .Select(s => new SessionBrief(
                s.Id,
                s.StartedAtUtc,
                s.FinishedAtUtc,
                s.TaskType,
                s.CorrectAnswers,
                s.WrongAnswers,
                s.TotalQuestions,
                s.Status))
            .ToListAsync(ct);

        var totalCorrect = await db.ExerciseAttempts.CountAsync(
            a => a.StudentProfileId == studentId && a.IsCorrect == true, ct);
        var totalAttempts = await db.ExerciseAttempts.CountAsync(
            a => a.StudentProfileId == studentId && a.IsCorrect != null, ct);

        return new StudentSummary(
            student.Id,
            student.Name,
            student.Grade,
            totalAttempts,
            totalCorrect,
            progress,
            achievements,
            recentSessions);
    }
}

public sealed record SessionBrief(
    Guid Id,
    DateTime StartedAtUtc,
    DateTime? FinishedAtUtc,
    TaskType TaskType,
    int CorrectAnswers,
    int WrongAnswers,
    int TotalQuestions,
    SessionStatus Status);

public sealed record StudentSummary(
    Guid StudentId,
    string Name,
    int Grade,
    int TotalAnswered,
    int TotalCorrect,
    IReadOnlyList<Domain.Entities.StudentTaskProgress> Progress,
    IReadOnlyList<Domain.Entities.StudentAchievement> Achievements,
    IReadOnlyList<SessionBrief> RecentSessions);
