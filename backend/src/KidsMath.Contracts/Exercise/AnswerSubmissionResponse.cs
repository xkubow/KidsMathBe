namespace KidsMath.Contracts.Exercise;

public record AnswerSubmissionResponse(
    int AttemptNumber,
    string Answer,
    bool IsCorrect,
    DateTime SubmittedAtUtc);
