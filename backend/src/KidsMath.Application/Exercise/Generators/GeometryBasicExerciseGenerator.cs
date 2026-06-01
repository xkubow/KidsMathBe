using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class GeometryBasicExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    private static readonly (string Cs, string En, int Sides)[] Shapes =
    [
        ("trojúhelník", "triangle", 3),
        ("čtverec", "square", 4),
        ("obdélník", "rectangle", 4),
        ("kruh", "circle", 0)
    ];

    public override bool Supports(TaskType taskType) => taskType == TaskType.GeometryBasic;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        var shape = Shapes[Random.Next(Shapes.Length)];
        if (shape.Sides == 0)
        {
            return new GeneratedExercise
            {
                QuestionTextCs = "Kolik rohů má kruh? (napiš 0)",
                QuestionTextEn = "How many corners does a circle have? (type 0)",
                CorrectAnswer = "0",
                QuestionData = new { shape = shape.En, sides = 0 }
            };
        }

        return new GeneratedExercise
        {
            QuestionTextCs = $"Kolik stran má {shape.Cs}?",
            QuestionTextEn = $"How many sides does a {shape.En} have?",
            CorrectAnswer = shape.Sides.ToString(),
            QuestionData = new { shape = shape.En, sides = shape.Sides }
        };
    }
}
