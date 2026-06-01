using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class EvenOddExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    public override bool Supports(TaskType taskType) => taskType == TaskType.EvenOdd;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        config = ParseConfig(definition);
        var n = PickNumber(config);
        var isEven = n % 2 == 0;
        var answerCs = isEven ? "sudé" : "liché";
        var answerEn = isEven ? "even" : "odd";

        return new GeneratedExercise
        {
            QuestionTextCs = $"Je číslo {n} sudé nebo liché? (napiš: sudé / liché)",
            QuestionTextEn = $"Is {n} even or odd? (type: even / odd)",
            CorrectAnswer = answerCs,
            QuestionData = new { number = n, expectedAnswerCs = answerCs, expectedAnswerEn = answerEn }
        };
    }
}
