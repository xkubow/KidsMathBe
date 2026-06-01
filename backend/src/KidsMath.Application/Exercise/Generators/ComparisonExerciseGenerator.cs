using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class ComparisonExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    public override bool Supports(TaskType taskType) => taskType == TaskType.Comparison;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        config = ParseConfig(definition);
        var left = PickNumber(config);
        var right = PickNumber(config);
        while (left == right)
        {
            right = PickNumber(config);
        }

        var answer = left > right ? ">" : "<";
        return new GeneratedExercise
        {
            QuestionTextCs = $"Které číslo je větší? {left} nebo {right} (napiš > nebo <)",
            QuestionTextEn = $"Which is greater? {left} or {right} (type > or <)",
            CorrectAnswer = answer,
            QuestionData = new { left, right, expectedAnswer = answer }
        };
    }
}
