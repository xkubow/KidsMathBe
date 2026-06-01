using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace KidsMath.Persistence;

public static class DatabaseInitializer
{
    public static async Task MigrateAndRepairAsync(KidsMathDbContext db, CancellationToken ct = default)
    {
        var databaseCreator = db.Database.GetService<IRelationalDatabaseCreator>();
        if (!await databaseCreator.ExistsAsync(ct))
        {
            await databaseCreator.CreateAsync(ct);
        }

        await db.Database.MigrateAsync(ct);
        await EnsureAnswerSubmissionsTableAsync(db, ct);
    }

    /// <summary>
    /// Repairs databases where an empty AddAnswerSubmissions migration was recorded without creating the table.
    /// </summary>
    private static async Task EnsureAnswerSubmissionsTableAsync(KidsMathDbContext db, CancellationToken ct)
    {
        await db.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS answer_submissions (
                "Id" uuid NOT NULL,
                "ExerciseAttemptId" uuid NOT NULL,
                "AttemptNumber" integer NOT NULL,
                "Answer" text NOT NULL,
                "IsCorrect" boolean NOT NULL,
                "SubmittedAtUtc" timestamp with time zone NOT NULL,
                CONSTRAINT "PK_answer_submissions" PRIMARY KEY ("Id"),
                CONSTRAINT "FK_answer_submissions_exercise_attempts_ExerciseAttemptId"
                    FOREIGN KEY ("ExerciseAttemptId")
                    REFERENCES exercise_attempts ("Id")
                    ON DELETE CASCADE
            );
            """,
            ct);

        await db.Database.ExecuteSqlRawAsync(
            """
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_answer_submissions_ExerciseAttemptId_AttemptNumber"
                ON answer_submissions ("ExerciseAttemptId", "AttemptNumber");
            """,
            ct);
    }
}
