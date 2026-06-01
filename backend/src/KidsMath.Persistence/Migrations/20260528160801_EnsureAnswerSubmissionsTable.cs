using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidsMath.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnsureAnswerSubmissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
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
                """);

            migrationBuilder.Sql("""
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_answer_submissions_ExerciseAttemptId_AttemptNumber"
                    ON answer_submissions ("ExerciseAttemptId", "AttemptNumber");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS answer_submissions;");
        }
    }
}
