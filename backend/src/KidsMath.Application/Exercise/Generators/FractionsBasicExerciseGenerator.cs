using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class FractionsBasicExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    private static readonly (string Cs, string En, string Value)[] Fractions =
    [
        ("polovina", "half", "1/2"),
        ("třetina", "third", "1/3"),
        ("čtvrtina", "quarter", "1/4")
    ];

    public override bool Supports(TaskType taskType) => taskType == TaskType.FractionsBasic;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        var whole = Random.Next(2, 9);
        var frac = Fractions[Random.Next(Fractions.Length)];
        return new GeneratedExercise
        {
            QuestionTextCs = $"Jaký je zlomek „{frac.Cs}“ z čísla {whole}? (např. 1/2)",
            QuestionTextEn = $"What fraction is \"{frac.En}\" of {whole}? (e.g. 1/2)",
            CorrectAnswer = frac.Value,
            QuestionData = new { whole, fraction = frac.Value }
        };
    }
}
