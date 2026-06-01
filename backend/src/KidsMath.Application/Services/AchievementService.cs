using System.Text.Json;
using KidsMath.Application.Abstractions;
using KidsMath.Application.Options;
using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KidsMath.Application.Services;

public class AchievementService(IKidsMathDbContext db, IOptions<AchievementOptions> options)
{
    public async Task EvaluateAfterAnswerAsync(Guid studentId, CancellationToken ct = default)
    {
        if (!options.Value.Enabled) return;

        var totalCorrect = await db.ExerciseAttempts.CountAsync(
            a => a.StudentProfileId == studentId && a.IsCorrect == true, ct);

        await TryUnlockAsync(studentId, "FIRST_TASK_SOLVED", _ => totalCorrect >= 1, ct);
        await TryUnlockAsync(studentId, "TEN_CORRECT_ANSWERS", _ => totalCorrect >= 10, ct);
        await TryUnlockAsync(studentId, "FIFTY_CORRECT_ANSWERS", _ => totalCorrect >= 50, ct);

        var lastAttempts = await db.ExerciseAttempts
            .Where(a => a.StudentProfileId == studentId && a.IsCorrect != null)
            .OrderByDescending(a => a.AnsweredAtUtc)
            .Take(5)
            .ToListAsync(ct);

        if (lastAttempts.Count >= 5 && lastAttempts.All(a => a.IsCorrect == true))
        {
            await TryUnlockAsync(studentId, "FIVE_CORRECT_IN_ROW", _ => true, ct);
        }
    }

    public async Task EvaluateAfterSessionAsync(ExerciseSession session, CancellationToken ct = default)
    {
        if (!options.Value.Enabled) return;

        await TryUnlockAsync(session.StudentProfileId, "FIRST_SESSION_FINISHED", _ => true, ct);

        if (session.WrongAnswers == 0 && session.CorrectAnswers == session.TotalQuestions && session.TotalQuestions > 0)
        {
            await TryUnlockAsync(session.StudentProfileId, "PERFECT_SESSION", _ => true, ct);
        }

        var taskTypeCode = session.TaskType switch
        {
            TaskType.Addition => "ADDITION_BEGINNER",
            TaskType.Subtraction => "SUBTRACTION_BEGINNER",
            TaskType.Multiplication => "MULTIPLICATION_BEGINNER",
            _ => null
        };

        if (taskTypeCode is not null)
        {
            var sessionIds = await db.ExerciseSessions
                .Where(s => s.StudentProfileId == session.StudentProfileId && s.TaskType == session.TaskType)
                .Select(s => s.Id)
                .ToListAsync(ct);
            var correctForType = await db.ExerciseAttempts.CountAsync(
                a => sessionIds.Contains(a.ExerciseSessionId) && a.IsCorrect == true, ct);
            await TryUnlockAsync(session.StudentProfileId, taskTypeCode, _ => correctForType >= 20, ct);
        }

        var practiceDays = await db.ExerciseSessions
            .Where(s => s.StudentProfileId == session.StudentProfileId && s.FinishedAtUtc != null)
            .Select(s => s.FinishedAtUtc!.Value.Date)
            .Distinct()
            .CountAsync(ct);

        if (practiceDays >= 3)
        {
            await TryUnlockAsync(session.StudentProfileId, "THREE_DAYS_PRACTICE", _ => true, ct);
        }
    }

    private async Task TryUnlockAsync(Guid studentId, string code, Func<Achievement, bool> condition, CancellationToken ct)
    {
        var achievement = await db.Achievements.FirstOrDefaultAsync(a => a.Code == code && a.IsActive, ct);
        if (achievement is null || !condition(achievement)) return;

        var exists = await db.StudentAchievements.AnyAsync(
            sa => sa.StudentProfileId == studentId && sa.AchievementId == achievement.Id, ct);
        if (exists) return;

        db.StudentAchievements.Add(new StudentAchievement
        {
            Id = Guid.NewGuid(),
            StudentProfileId = studentId,
            AchievementId = achievement.Id,
            UnlockedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<StudentAchievement>> GetStudentAchievementsAsync(Guid studentId, CancellationToken ct = default) =>
        await db.StudentAchievements.AsNoTracking()
            .Include(sa => sa.Achievement)
            .Where(sa => sa.StudentProfileId == studentId)
            .OrderByDescending(sa => sa.UnlockedAtUtc)
            .ToListAsync(ct);
}
