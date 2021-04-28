using Microsoft.EntityFrameworkCore.Migrations;

namespace QMS_API.Data.Migrations
{
    public partial class AddAnswerTableChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_Answers_GivenAnswerId",
                table: "QuizAnswer");

            migrationBuilder.DropIndex(
                name: "IX_QuizAnswer_GivenAnswerId",
                table: "QuizAnswer");

            migrationBuilder.DropColumn(
                name: "GivenAnswerId",
                table: "QuizAnswer");

            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "QuizAnswer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchingText",
                table: "QuizAnswer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "QuizAnswer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "QuizAnswer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswer_QuestionId",
                table: "QuizAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswer_TestId",
                table: "QuizAnswer",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_Questions_QuestionId",
                table: "QuizAnswer",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_Tests_TestId",
                table: "QuizAnswer",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_Questions_QuestionId",
                table: "QuizAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_Tests_TestId",
                table: "QuizAnswer");

            migrationBuilder.DropIndex(
                name: "IX_QuizAnswer_QuestionId",
                table: "QuizAnswer");

            migrationBuilder.DropIndex(
                name: "IX_QuizAnswer_TestId",
                table: "QuizAnswer");

            migrationBuilder.DropColumn(
                name: "Answer",
                table: "QuizAnswer");

            migrationBuilder.DropColumn(
                name: "MatchingText",
                table: "QuizAnswer");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "QuizAnswer");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "QuizAnswer");

            migrationBuilder.AddColumn<int>(
                name: "GivenAnswerId",
                table: "QuizAnswer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswer_GivenAnswerId",
                table: "QuizAnswer",
                column: "GivenAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_Answers_GivenAnswerId",
                table: "QuizAnswer",
                column: "GivenAnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
