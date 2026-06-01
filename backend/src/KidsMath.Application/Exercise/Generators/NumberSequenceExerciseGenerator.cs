using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise.Generators;

public sealed class NumberSequenceExerciseGenerator(IRandomNumberSource random)
    : ExerciseGeneratorBase(random)
{
    public override bool Supports(TaskType taskType) => taskType == TaskType.NumberSequence;

    public override GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config)
    {
        config = ParseConfig(definition);
        var step = config.SequenceStep > 0 ? config.SequenceStep : 2;
        var start = PickNumber(config);
        var seq = new[] { start, start + step, start + step * 2 };
        var answer = (start + step * 3).ToString();

        return new GeneratedExercise
        {
            QuestionTextCs = $"Doplň číslo: {seq[0]}, {seq[1]}, {seq[2]}, ?",
            QuestionTextEn = $"Fill in the number: {seq[0]}, {seq[1]}, {seq[2]}, ?",
            CorrectAnswer = answer,
            QuestionData = new { sequence = seq, step, expectedAnswer = int.Parse(answer) }
        };
    }
}
