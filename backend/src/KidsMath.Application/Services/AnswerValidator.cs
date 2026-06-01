namespace KidsMath.Application.Services;

public static class AnswerValidator
{
    private static readonly Dictionary<string, string> EvenOddAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["sudé"] = "sudé",
        ["liche"] = "liché",
        ["liché"] = "liché",
        ["even"] = "even",
        ["odd"] = "odd"
    };

    public static bool IsCorrect(string correctAnswer, string studentAnswer, string? taskTypeHint = null)
    {
        var normalizedStudent = Normalize(studentAnswer);
        var normalizedCorrect = Normalize(correctAnswer);

        if (string.Equals(normalizedStudent, normalizedCorrect, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (EvenOddAliases.TryGetValue(normalizedStudent, out var aliasStudent)
            && EvenOddAliases.TryGetValue(normalizedCorrect, out var aliasCorrect))
        {
            var evenGroup = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "sudé", "even" };
            var oddGroup = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "liché", "odd" };
            if (evenGroup.Contains(aliasStudent) && evenGroup.Contains(aliasCorrect)) return true;
            if (oddGroup.Contains(aliasStudent) && oddGroup.Contains(aliasCorrect)) return true;
        }

        if (decimal.TryParse(normalizedStudent, out var s) && decimal.TryParse(normalizedCorrect, out var c))
        {
            return s == c;
        }

        return false;
    }

    private static string Normalize(string value) =>
        value.Trim().Replace(" ", "").Replace(",", ".");
}
