using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidsMath.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplateThemeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TemplateThemeId",
                table: "exercise_sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TemplateThemeId",
                table: "exercise_attempts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateThemeId",
                table: "exercise_sessions");

            migrationBuilder.DropColumn(
                name: "TemplateThemeId",
                table: "exercise_attempts");
        }
    }
}
