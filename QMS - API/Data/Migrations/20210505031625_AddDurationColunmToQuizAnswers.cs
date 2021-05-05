using Microsoft.EntityFrameworkCore.Migrations;

namespace QMS_API.Data.Migrations
{
    public partial class AddDurationColunmToQuizAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "QuizAnswers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "QuizAnswers");
        }
    }
}
