using KidsMath.Domain.Entities;

namespace KidsMath.Application.Services;

public sealed class SubmitAnswerResult
{
    public required ExerciseAttempt Attempt { get; init; }
    public required bool SubmissionIsCorrect { get; init; }
    public required bool QuestionResolved { get; init; }
    public required int AttemptsUsed { get; init; }
    public required int MaxAttempts { get; init; }
}
