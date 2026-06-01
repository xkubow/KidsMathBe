using KidsMath.Application.Abstractions;
using KidsMath.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KidsMath.Application.Services;

public class ProgressService(IKidsMathDbContext db)
{
    public async Task UpdateAfterQuestionResolvedAsync(ExerciseAttempt attempt, CancellationToken ct = default)
    {
        var session = await db.ExerciseSessions.AsNoTracking()
            .FirstAsync(s => s.Id == attempt.ExerciseSessionId, ct);

        var progress = await GetOrCreateProgressAsync(attempt.StudentProfileId, session.Grade, session.TaskType, session.DifficultyLevel, ct);
        progress.TotalAttempts++;
        if (attempt.IsCorrect == true)
        {
            progress.CorrectAttempts++;
            progress.CurrentStreak++;
        }
        else
        {
            progress.WrongAttempts++;
            progress.CurrentStreak = 0;
        }

        progress.LastPracticedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAfterSessionAsync(ExerciseSession session, CancellationToken ct = default)
    {
        var progress = await GetOrCreateProgressAsync(session.StudentProfileId, session.Grade, session.TaskType, session.DifficultyLevel, ct);
        var score = session.TotalQuestions > 0
            ? (int)Math.Round(100.0 * session.CorrectAnswers / session.TotalQuestions)
            : 0;
        if (score > progress.BestScore)
        {
            progress.BestScore = score;
        }

        progress.LastPracticedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<StudentTaskProgress>> GetProgressAsync(Guid studentId, CancellationToken ct = default) =>
        await db.StudentTaskProgress.AsNoTracking()
            .Where(p => p.StudentProfileId == studentId)
            .OrderBy(p => p.Grade).ThenBy(p => p.TaskType)
            .ToListAsync(ct);

    private async Task<StudentTaskProgress> GetOrCreateProgressAsync(
        Guid studentId, int grade, Domain.Enums.TaskType taskType, int difficulty, CancellationToken ct)
    {
        var progress = await db.StudentTaskProgress
            .FirstOrDefaultAsync(p =>
                p.StudentProfileId == studentId &&
                p.Grade == grade &&
                p.TaskType == taskType &&
                p.DifficultyLevel == difficulty, ct);

        if (progress is not null) return progress;

        progress = new StudentTaskProgress
        {
            Id = Guid.NewGuid(),
            StudentProfileId = studentId,
            Grade = grade,
            TaskType = taskType,
            DifficultyLevel = difficulty
        };
        db.StudentTaskProgress.Add(progress);
        return progress;
    }
}
