namespace KidsMath.Contracts.Exercise;

public record SubmitAnswerResponse(
    Guid Id,
    bool IsCorrect,
    bool QuestionResolved,
    int AttemptsUsed,
    int MaxAttempts,
    string? StudentAnswer,
    string? CorrectAnswer,
    bool? FinalOutcome);
