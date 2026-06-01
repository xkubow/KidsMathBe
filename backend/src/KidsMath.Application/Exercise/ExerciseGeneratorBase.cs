using System.Text.Json;
using KidsMath.Domain.Entities;

namespace KidsMath.Application.Exercise;

public abstract class ExerciseGeneratorBase(IRandomNumberSource random) : IExerciseGenerator
{
    protected IRandomNumberSource Random { get; } = random;

    public abstract GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config);

    public abstract bool Supports(Domain.Enums.TaskType taskType);

    protected static TaskConfig ParseConfig(MathTaskDefinition definition)
    {
        return JsonSerializer.Deserialize<TaskConfig>(definition.ConfigJson, JsonSerializerOptions.Web)
               ?? new TaskConfig();
    }

    protected int PickNumber(TaskConfig config) => Random.Next(config.MinNumber, config.MaxNumber + 1);
}
