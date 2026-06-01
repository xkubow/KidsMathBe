using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidsMath.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnswerSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "answer_submissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseAttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    SubmittedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answer_submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_answer_submissions_exercise_attempts_ExerciseAttemptId",
                        column: x => x.ExerciseAttemptId,
                        principalTable: "exercise_attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_answer_submissions_ExerciseAttemptId_AttemptNumber",
                table: "answer_submissions",
                columns: new[] { "ExerciseAttemptId", "AttemptNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answer_submissions");
        }
    }
}
