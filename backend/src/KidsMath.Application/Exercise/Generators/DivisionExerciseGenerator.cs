using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class DivisionExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    public override bool Supports(TaskType taskType) => taskType == TaskType.Division;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        config = ParseConfig(definition);
        var divisor = Random.Next(Math.Max(1, config.MinNumber), config.MaxNumber + 1);
        var quotient = Random.Next(1, 11);
        var dividend = divisor * quotient;

        return new GeneratedExercise
        {
            QuestionTextCs = $"Kolik je {dividend} ÷ {divisor}?",
            QuestionTextEn = $"What is {dividend} ÷ {divisor}?",
            CorrectAnswer = quotient.ToString(),
            QuestionData = new { dividend, divisor, expectedAnswer = quotient }
        };
    }
}
