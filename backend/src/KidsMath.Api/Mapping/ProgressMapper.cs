using KidsMath.Application.Services;
using KidsMath.Contracts.Progress;
using KidsMath.Domain.Entities;

namespace KidsMath.Api.Mapping;

public static class ProgressMapper
{
    public static StudentTaskProgressResponse ToResponse(StudentTaskProgress progress) =>
        new(
            progress.Grade,
            progress.TaskType,
            progress.DifficultyLevel,
            progress.TotalAttempts,
            progress.CorrectAttempts,
            progress.WrongAttempts,
            progress.BestScore,
            progress.CurrentStreak,
            progress.LastPracticedAtUtc);

    public static RecentSessionResponse ToRecentSessionResponse(SessionBrief session) =>
        new(
            session.Id,
            session.StartedAtUtc,
            session.FinishedAtUtc,
            session.TaskType,
            session.CorrectAnswers,
            session.WrongAnswers,
            session.TotalQuestions,
            session.Status);

    public static StudentSummaryResponse ToSummaryResponse(StudentSummary summary, string lang) =>
        new(
            summary.StudentId,
            summary.Name,
            summary.Grade,
            summary.TotalAnswered,
            summary.TotalCorrect,
            summary.Progress.Select(ToResponse).ToList(),
            summary.Achievements.Select(a => AchievementMapper.ToStudentResponse(a, lang)).ToList(),
            summary.RecentSessions.Select(ToRecentSessionResponse).ToList());
}
