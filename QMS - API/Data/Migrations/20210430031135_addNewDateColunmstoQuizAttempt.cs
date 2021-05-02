using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QMS_API.Data.Migrations
{
    public partial class addNewDateColunmstoQuizAttempt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "QuizAttempts");

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishDate",
                table: "QuizAttempts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "QuizAttempts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishDate",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "QuizAttempts");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "QuizAttempts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
