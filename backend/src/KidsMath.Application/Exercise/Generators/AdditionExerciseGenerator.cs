using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class AdditionExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    public override bool Supports(TaskType taskType) => taskType == TaskType.Addition;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        config = definition.ConfigJson.Length > 2 ? ParseConfig(definition) : config;
        var left = PickNumber(config);
        var right = PickNumber(config);
        if (!config.AllowCarry)
        {
            while (left % 10 + right % 10 >= 10)
            {
                left = PickNumber(config);
                right = PickNumber(config);
            }
        }

        var sum = left + right;
        if (!config.AllowNegativeResult && sum < 0)
        {
            (left, right) = (right, left);
            sum = left + right;
        }

        return new GeneratedExercise
        {
            QuestionTextCs = $"Kolik je {left} + {right}?",
            QuestionTextEn = $"What is {left} + {right}?",
            CorrectAnswer = sum.ToString(),
            QuestionData = new { left, right, @operator = "+", expectedAnswer = sum }
        };
    }
}
