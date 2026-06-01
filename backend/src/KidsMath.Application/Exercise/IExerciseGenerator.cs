using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise;

public interface IExerciseGenerator
{
    GeneratedExercise Generate(MathTaskDefinition definition, TaskConfig config);
    bool Supports(TaskType taskType);
}
