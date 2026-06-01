using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class MissingNumberExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    public override bool Supports(TaskType taskType) => taskType == TaskType.MissingNumber;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        config = ParseConfig(definition);
        var a = PickNumber(config);
        var b = PickNumber(config);
        var sum = a + b;
        var hideFirst = Random.Next(2) == 0;

        if (hideFirst)
        {
            return new GeneratedExercise
            {
                QuestionTextCs = $"? + {b} = {sum}",
                QuestionTextEn = $"? + {b} = {sum}",
                CorrectAnswer = a.ToString(),
                QuestionData = new { missing = "left", a, b, sum }
            };
        }

        return new GeneratedExercise
        {
            QuestionTextCs = $"{a} + ? = {sum}",
            QuestionTextEn = $"{a} + ? = {sum}",
            CorrectAnswer = b.ToString(),
            QuestionData = new { missing = "right", a, b, sum }
        };
    }
}
