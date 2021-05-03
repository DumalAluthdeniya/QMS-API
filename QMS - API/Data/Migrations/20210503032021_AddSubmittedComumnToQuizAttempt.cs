using Microsoft.EntityFrameworkCore.Migrations;

namespace QMS_API.Data.Migrations
{
    public partial class AddSubmittedComumnToQuizAttempt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Submitted",
                table: "QuizAttempts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Submitted",
                table: "QuizAttempts");
        }
    }
}
