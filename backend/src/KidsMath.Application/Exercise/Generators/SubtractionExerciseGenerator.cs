using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class SubtractionExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    public override bool Supports(TaskType taskType) => taskType == TaskType.Subtraction;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        config = ParseConfig(definition);
        var left = PickNumber(config);
        var right = PickNumber(config);
        if (!config.AllowNegativeResult && right > left)
        {
            (left, right) = (right, left);
        }

        if (!config.AllowBorrow)
        {
            while (left % 10 < right % 10)
            {
                left = PickNumber(config);
                right = PickNumber(config);
                if (!config.AllowNegativeResult && right > left)
                {
                    (left, right) = (right, left);
                }
            }
        }

        var result = left - right;
        return new GeneratedExercise
        {
            QuestionTextCs = $"Kolik je {left} − {right}?",
            QuestionTextEn = $"What is {left} − {right}?",
            CorrectAnswer = result.ToString(),
            QuestionData = new { left, right, @operator = "-", expectedAnswer = result }
        };
    }
}
