using System.Globalization;
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
        ("čtvrtina", "quarter", "1/4"),
        ("pětina", "fifth", "1/5"),
        ("šestina", "sixth", "1/6"),
        ("sedmina", "seventh", "1/7"),
        ("osmina", "eighth", "1/8")
    ];

    public override bool Supports(TaskType taskType) => taskType == TaskType.FractionsBasic;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        config = definition.ConfigJson.Length > 2 ? ParseConfig(definition) : config;

        var fractionOptions = (config.Fractions is { Length: > 0 })
            ? config.Fractions
            : Fractions.Select(x => x.Value).ToArray();

        var fractionValue = fractionOptions[Random.Next(fractionOptions.Length)];
        if (!TryParseFraction(fractionValue, out var numerator, out var denominator) || denominator <= 0)
        {
            // Fallback to "1/2" if config is invalid.
            numerator = 1;
            denominator = 2;
            fractionValue = "1/2";
        }

        var (csName, enName) = GetFractionName(fractionValue);
        var whole = Random.Next(config.MinNumber, config.MaxNumber +1); //PickWholeNumberMultiple(config, denominator);
        var result = Math.Round(whole * numerator / (double)denominator, 2);
        return new GeneratedExercise
        {
            QuestionTextCs = $"Kolik je {csName} z čísla {whole}?",
            QuestionTextEn = $"What is {enName} of {whole}?",
            CorrectAnswer = result.ToString(CultureInfo.InvariantCulture),
            QuestionData = new { whole, fraction = fractionValue, expectedAnswer = result }
        };
    }

    private int PickWholeNumberMultiple(TaskConfig config, int denominator)
    {
        var min = Math.Max(1, config.MinNumber);
        var max = Math.Max(min, config.MaxNumber);

        var kMin = (int)Math.Ceiling(min / (double)denominator);
        var kMax = max / denominator;
        if (kMax < kMin)
        {
            // If the configured range can't produce an integer result, fall back to a safe range.
            kMin = 1;
            kMax = 12;
        }

        var k = Random.Next(kMin, kMax + 1);
        return k * denominator;
    }

    private static bool TryParseFraction(string value, out int numerator, out int denominator)
    {
        numerator = 0;
        denominator = 0;
        var parts = value.Trim().Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length == 2
               && int.TryParse(parts[0], out numerator)
               && int.TryParse(parts[1], out denominator);
    }

    private static (string Cs, string En) GetFractionName(string fractionValue)
    {
        var known = Fractions.FirstOrDefault(x => x.Value.Equals(fractionValue, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(known.Cs)) return (known.Cs, known.En);
        return (fractionValue, fractionValue);
    }
}
