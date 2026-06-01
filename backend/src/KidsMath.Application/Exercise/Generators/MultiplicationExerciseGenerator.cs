using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class MultiplicationExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    public override bool Supports(TaskType taskType) => taskType == TaskType.Multiplication;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        config = ParseConfig(definition);
        var multipliers = config.Multipliers ?? Enumerable.Range(1, 10).ToArray();
        var left = multipliers[Random.Next(multipliers.Length)];
        var right = Random.Next(config.MinNumber == 0 ? 1 : config.MinNumber, config.MaxNumber + 1);
        if (right == 0) right = 1;
        var product = left * right;

        return new GeneratedExercise
        {
            QuestionTextCs = $"Kolik je {left} × {right}?",
            QuestionTextEn = $"What is {left} × {right}?",
            CorrectAnswer = product.ToString(),
            QuestionData = new { left, right, @operator = "*", expectedAnswer = product }
        };
    }
}
