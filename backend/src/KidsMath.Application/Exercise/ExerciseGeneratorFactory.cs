using System.Text.Json;
using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise;

public sealed class ExerciseGeneratorFactory(IEnumerable<IExerciseGenerator> generators)
{
    public GeneratedExercise Generate(MathTaskDefinition definition)
    {
        var config = JsonSerializer.Deserialize<TaskConfig>(definition.ConfigJson, JsonSerializerOptions.Web)
                     ?? new TaskConfig();
        var generator = generators.FirstOrDefault(g => g.Supports(definition.TaskType))
                        ?? throw new InvalidOperationException($"No generator for {definition.TaskType}");
        return generator.Generate(definition, config);
    }
}
