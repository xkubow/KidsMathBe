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

        if (TryParseNumeric(normalizedStudent, out var s) && TryParseNumeric(normalizedCorrect, out var c))
        {
            return Math.Abs(s - c) < 1e-2;
        }

        return false;
    }

    private static bool TryParseNumeric(string value, out double result)
    {
        if (double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result))
        {
            return true;
        }

        var parts = value.Split('/');
        if (parts.Length == 2 &&
            double.TryParse(parts[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var numerator) &&
            double.TryParse(parts[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var denominator) &&
            denominator != 0)
        {
            result = numerator / denominator;
            return true;
        }

        result = 0;
        return false;
    }

    private static string Normalize(string value) =>
        value.Trim().Replace(" ", "").Replace(",", ".");
}
