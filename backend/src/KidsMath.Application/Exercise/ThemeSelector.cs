using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;

namespace KidsMath.Application.Exercise;

public static class ThemeSelector
{
    private static readonly TemplateTheme[] ThemedOptions =
    [
        TemplateTheme.Space,
        TemplateTheme.Pirates,
        TemplateTheme.Animals
    ];

    public static TemplateTheme PickForStudent(StudentProfile student, IRandomNumberSource random)
    {
        if (!string.IsNullOrWhiteSpace(student.AvatarKey))
        {
            var themed = AvatarThemeMap.TryGetValue(student.AvatarKey, out var mapped)
                ? mapped
                : ThemedOptions[Math.Abs(student.AvatarKey.GetHashCode()) % ThemedOptions.Length];
            return themed;
        }

        return ThemedOptions[random.Next(ThemedOptions.Length)];
    }

    private static readonly Dictionary<string, TemplateTheme> AvatarThemeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["fox"] = TemplateTheme.Animals,
        ["cat"] = TemplateTheme.Animals,
        ["bear"] = TemplateTheme.Animals,
        ["rabbit"] = TemplateTheme.Animals,
        ["owl"] = TemplateTheme.Animals,
        ["frog"] = TemplateTheme.Animals
    };
}
