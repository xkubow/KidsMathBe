using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise;

public static class QuestionThemeFormatter
{
    public static GeneratedExercise ApplyTheme(TemplateTheme theme, GeneratedExercise exercise)
    {
        return new GeneratedExercise
        {
            QuestionTextCs = exercise.QuestionTextCs,
            QuestionTextEn = exercise.QuestionTextEn,
            CorrectAnswer = exercise.CorrectAnswer,
            QuestionData = exercise.QuestionData,
            Theme = theme
        };
    }
}
