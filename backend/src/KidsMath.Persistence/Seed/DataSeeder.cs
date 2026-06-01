using KidsMath.Domain.Entities;
using KidsMath.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KidsMath.Persistence.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(KidsMathDbContext db, CancellationToken ct = default)
    {
        if (!await db.Achievements.AnyAsync(ct))
        {
            db.Achievements.AddRange(CreateAchievements());
        }

        if (!await db.MathTaskDefinitions.AnyAsync(ct))
        {
            db.MathTaskDefinitions.AddRange(CreateTaskDefinitions());
        }

        await db.SaveChangesAsync(ct);
    }

    private static IEnumerable<Achievement> CreateAchievements() =>
    [
        Ach("FIRST_TASK_SOLVED", "První úkol", "First task", "Vyřešil jsi první úlohu!", "You solved your first task!"),
        Ach("FIRST_SESSION_FINISHED", "První lekce", "First session", "Dokončil jsi první cvičení.", "You finished your first practice."),
        Ach("TEN_CORRECT_ANSWERS", "Desítka", "Ten correct", "10 správných odpovědí!", "10 correct answers!"),
        Ach("FIFTY_CORRECT_ANSWERS", "Padesátka", "Fifty correct", "50 správných odpovědí!", "50 correct answers!"),
        Ach("FIVE_CORRECT_IN_ROW", "Pět za sebou", "Five in a row", "5 správných odpovědí po sobě!", "5 correct in a row!"),
        Ach("PERFECT_SESSION", "Bez chyby", "Perfect session", "Celé cvičení bez chyby!", "A perfect session!"),
        Ach("ADDITION_BEGINNER", "Sčítání", "Addition beginner", "20 správných sečtení.", "20 correct additions."),
        Ach("SUBTRACTION_BEGINNER", "Odčítání", "Subtraction beginner", "20 správných odčítání.", "20 correct subtractions."),
        Ach("MULTIPLICATION_BEGINNER", "Násobení", "Multiplication beginner", "20 správných násobení.", "20 correct multiplications."),
        Ach("THREE_DAYS_PRACTICE", "Tři dny", "Three days", "Cvičil jsi 3 různé dny.", "You practiced on 3 different days.")
    ];

    private static Achievement Ach(string code, string nameCs, string nameEn, string descCs, string descEn) => new()
    {
        Id = Guid.NewGuid(),
        Code = code,
        DisplayNameCs = nameCs,
        DisplayNameEn = nameEn,
        DescriptionCs = descCs,
        DescriptionEn = descEn,
        ConditionType = "simple",
        ConditionJson = "{}",
        IsActive = true
    };

    private static IEnumerable<MathTaskDefinition> CreateTaskDefinitions()
    {
        var now = DateTime.UtcNow;
        MathTaskDefinition Def(int grade, TaskType type, int diff, string cs, string en, string json) => new()
        {
            Id = Guid.NewGuid(),
            Grade = grade,
            TaskType = type,
            DifficultyLevel = diff,
            DisplayNameCs = cs,
            DisplayNameEn = en,
            ConfigJson = json,
            IsActive = true,
            CreatedAtUtc = now
        };

        return
        [
            // Grade 1
            Def(1, TaskType.Addition, 1, "Sčítání 0–10", "Addition 0–10", """{"minNumber":0,"maxNumber":10,"allowCarry":false}"""),
            Def(1, TaskType.Addition, 2, "Sčítání 0–20", "Addition 0–20", """{"minNumber":0,"maxNumber":20,"allowCarry":true}"""),
            Def(1, TaskType.Subtraction, 1, "Odčítání 0–10", "Subtraction 0–10", """{"minNumber":0,"maxNumber":10,"allowBorrow":false}"""),
            Def(1, TaskType.Subtraction, 2, "Odčítání 0–20", "Subtraction 0–20", """{"minNumber":0,"maxNumber":20,"allowBorrow":true}"""),
            Def(1, TaskType.Comparison, 1, "Porovnání 0–20", "Comparison 0–20", """{"minNumber":0,"maxNumber":20}"""),
            Def(1, TaskType.MissingNumber, 1, "Doplň číslo 0–10", "Missing number 0–10", """{"minNumber":0,"maxNumber":10}"""),
            // Grade 2
            Def(2, TaskType.Addition, 1, "Sčítání do 100 bez přenosu", "Addition to 100 no carry", """{"minNumber":0,"maxNumber":100,"allowCarry":false}"""),
            Def(2, TaskType.Addition, 2, "Sčítání do 100 s přenosem", "Addition to 100 with carry", """{"minNumber":0,"maxNumber":100,"allowCarry":true}"""),
            Def(2, TaskType.Subtraction, 1, "Odčítání do 100", "Subtraction to 100", """{"minNumber":0,"maxNumber":100,"allowBorrow":false}"""),
            Def(2, TaskType.Subtraction, 2, "Odčítání s půjčkou", "Subtraction with borrow", """{"minNumber":0,"maxNumber":100,"allowBorrow":true}"""),
            Def(2, TaskType.Multiplication, 1, "Násobení 2, 5, 10", "Multiply by 2, 5, 10", """{"minNumber":1,"maxNumber":10,"multipliers":[2,5,10]}"""),
            Def(2, TaskType.EvenOdd, 1, "Sudá a lichá", "Even and odd", """{"minNumber":0,"maxNumber":100}"""),
            Def(2, TaskType.NumberSequence, 1, "Číselná řada", "Number sequence", """{"minNumber":0,"maxNumber":50,"sequenceStep":2}"""),
            // Grade 3
            Def(3, TaskType.Multiplication, 1, "Malá násobilka", "Times tables 1–10", """{"minNumber":1,"maxNumber":10,"multipliers":[1,2,3,4,5,6,7,8,9,10]}"""),
            Def(3, TaskType.Division, 1, "Dělení základy", "Division basics", """{"minNumber":1,"maxNumber":10}"""),
            Def(3, TaskType.Addition, 2, "Sčítání do 1000", "Addition to 1000", """{"minNumber":0,"maxNumber":1000,"allowCarry":true}"""),
            Def(3, TaskType.Subtraction, 2, "Odčítání do 1000", "Subtraction to 1000", """{"minNumber":0,"maxNumber":1000,"allowBorrow":true}"""),
            Def(3, TaskType.FractionsBasic, 1, "Zlomky základy", "Fractions basics", """{"minNumber":1,"maxNumber":12}"""),
            Def(3, TaskType.GeometryBasic, 1, "Tvary", "Shapes", "{}")
        ];
    }
}
