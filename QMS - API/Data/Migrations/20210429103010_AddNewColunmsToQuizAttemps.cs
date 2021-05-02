using Microsoft.EntityFrameworkCore.Migrations;

namespace QMS_API.Data.Migrations
{
    public partial class AddNewColunmsToQuizAttemps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "QuizAttempts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Percentage",
                table: "QuizAttempts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "QuizAttempts",
                type: "decimal(10, 2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "QuizAttempts");
        }
    }
}
