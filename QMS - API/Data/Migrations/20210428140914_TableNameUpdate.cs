using Microsoft.EntityFrameworkCore.Migrations;

namespace QMS_API.Data.Migrations
{
    public partial class TableNameUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_Questions_QuestionId",
                table: "QuizAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_QuizAttempt_QuizAttemptId",
                table: "QuizAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswer_Tests_TestId",
                table: "QuizAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempt_Links_LinkId",
                table: "QuizAttempt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizAttempt",
                table: "QuizAttempt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizAnswer",
                table: "QuizAnswer");

            migrationBuilder.RenameTable(
                name: "QuizAttempt",
                newName: "QuizAttempts");

            migrationBuilder.RenameTable(
                name: "QuizAnswer",
                newName: "QuizAnswers");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAttempt_LinkId",
                table: "QuizAttempts",
                newName: "IX_QuizAttempts_LinkId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswer_TestId",
                table: "QuizAnswers",
                newName: "IX_QuizAnswers_TestId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswer_QuizAttemptId",
                table: "QuizAnswers",
                newName: "IX_QuizAnswers_QuizAttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswer_QuestionId",
                table: "QuizAnswers",
                newName: "IX_QuizAnswers_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizAttempts",
                table: "QuizAttempts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizAnswers",
                table: "QuizAnswers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswers_Questions_QuestionId",
                table: "QuizAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswers_QuizAttempts_QuizAttemptId",
                table: "QuizAnswers",
                column: "QuizAttemptId",
                principalTable: "QuizAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswers_Tests_TestId",
                table: "QuizAnswers",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_Links_LinkId",
                table: "QuizAttempts",
                column: "LinkId",
                principalTable: "Links",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswers_Questions_QuestionId",
                table: "QuizAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswers_QuizAttempts_QuizAttemptId",
                table: "QuizAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAnswers_Tests_TestId",
                table: "QuizAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_Links_LinkId",
                table: "QuizAttempts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizAttempts",
                table: "QuizAttempts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizAnswers",
                table: "QuizAnswers");

            migrationBuilder.RenameTable(
                name: "QuizAttempts",
                newName: "QuizAttempt");

            migrationBuilder.RenameTable(
                name: "QuizAnswers",
                newName: "QuizAnswer");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAttempts_LinkId",
                table: "QuizAttempt",
                newName: "IX_QuizAttempt_LinkId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswers_TestId",
                table: "QuizAnswer",
                newName: "IX_QuizAnswer_TestId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswers_QuizAttemptId",
                table: "QuizAnswer",
                newName: "IX_QuizAnswer_QuizAttemptId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizAnswers_QuestionId",
                table: "QuizAnswer",
                newName: "IX_QuizAnswer_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizAttempt",
                table: "QuizAttempt",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizAnswer",
                table: "QuizAnswer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_Questions_QuestionId",
                table: "QuizAnswer",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_QuizAttempt_QuizAttemptId",
                table: "QuizAnswer",
                column: "QuizAttemptId",
                principalTable: "QuizAttempt",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAnswer_Tests_TestId",
                table: "QuizAnswer",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempt_Links_LinkId",
                table: "QuizAttempt",
                column: "LinkId",
                principalTable: "Links",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
