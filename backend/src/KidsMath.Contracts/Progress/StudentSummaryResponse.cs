namespace KidsMath.Contracts.Progress;

public record StudentSummaryResponse(
    Guid StudentId,
    string Name,
    int Grade,
    int TotalAnswered,
    int TotalCorrect,
    IReadOnlyList<StudentTaskProgressResponse> Progress,
    IReadOnlyList<Achievements.StudentAchievementResponse> Achievements,
    IReadOnlyList<RecentSessionResponse> RecentSessions);
