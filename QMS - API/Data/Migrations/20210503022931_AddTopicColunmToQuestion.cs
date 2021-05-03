using Microsoft.EntityFrameworkCore.Migrations;

namespace QMS_API.Data.Migrations
{
    public partial class AddTopicColunmToQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "Questions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Topic",
                table: "Questions");
        }
    }
}
