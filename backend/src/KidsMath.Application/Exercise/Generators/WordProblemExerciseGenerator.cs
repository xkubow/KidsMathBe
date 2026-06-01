using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class WordProblemExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    public override bool Supports(TaskType taskType) => taskType == TaskType.WordProblem;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        config = ParseConfig(definition);
        var apples = PickNumber(config);
        var eaten = Random.Next(1, apples + 1);
        var left = apples - eaten;

        return new GeneratedExercise
        {
            QuestionTextCs = $"Máš {apples} jablek a sníš {eaten}. Kolik jablek zbude?",
            QuestionTextEn = $"You have {apples} apples and eat {eaten}. How many are left?",
            CorrectAnswer = left.ToString(),
            QuestionData = new { apples, eaten, left }
        };
    }
}
