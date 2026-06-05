using System.Text.Json;
using KidsMath.Domain.Entities;

namespace KidsMath.Application.Exercise;

/// <summary>
/// Builds a session question list: for each slot, rolls static chance; on success picks an unused static exercise, otherwise generates one.
/// </summary>
public sealed class SessionQuestionGenerator(
    IEnumerable<IExerciseGenerator> generators,
    IRandomNumberSource random)
{
    public IReadOnlyList<GeneratedExercise> Generate(MathTaskDefinition definition, int count)
    {
        var config = JsonSerializer.Deserialize<TaskConfig>(definition.ConfigJson, JsonSerializerOptions.Web)
                     ?? new TaskConfig();
        var generator = generators.FirstOrDefault(g => g.Supports(definition.TaskType))
                        ?? throw new InvalidOperationException($"No generator for {definition.TaskType}");

        var staticPool = config.StaticExercises ?? [];
        var chancePercent = Math.Clamp(config.StaticExerciseChancePercent, 0, 100);
        var usedStaticIndices = new HashSet<int>();
        var questions = new List<GeneratedExercise>(count);

        for (var i = 0; i < count; i++)
        {
            if (TryPickStatic(staticPool, usedStaticIndices, chancePercent, out var staticExercise))
            {
                questions.Add(staticExercise);
            }
            else
            {
                questions.Add(generator.Generate(definition, config));
            }
        }

        return questions;
    }

    private bool TryPickStatic(
        StaticExerciseConfig[] pool,
        HashSet<int> usedIndices,
        int chancePercent,
        out GeneratedExercise exercise)
    {
        exercise = null!;

        if (chancePercent <= 0 || pool.Length == 0)
        {
            return false;
        }

        if (random.Next(100) >= chancePercent)
        {
            return false;
        }

        var available = new List<int>();
        for (var i = 0; i < pool.Length; i++)
        {
            if (!usedIndices.Contains(i))
            {
                available.Add(i);
            }
        }

        if (available.Count == 0)
        {
            return false;
        }

        var index = available[random.Next(available.Count)];
        usedIndices.Add(index);

        var picked = pool[index];
        if (string.IsNullOrWhiteSpace(picked.CorrectAnswer))
        {
            usedIndices.Remove(index);
            return false;
        }

        exercise = new GeneratedExercise
        {
            QuestionTextCs = picked.QuestionTextCs,
            QuestionTextEn = picked.QuestionTextEn,
            CorrectAnswer = picked.CorrectAnswer,
            QuestionData = picked.QuestionData.HasValue ? picked.QuestionData.Value : new { }
        };
        return true;
    }
}
