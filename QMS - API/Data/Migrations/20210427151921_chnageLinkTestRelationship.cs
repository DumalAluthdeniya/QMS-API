using Microsoft.EntityFrameworkCore.Migrations;

namespace QMS_API.Data.Migrations
{
    public partial class chnageLinkTestRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Links_LinkId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_LinkId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "LinkId",
                table: "Tests");

            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "Links",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Links_TestId",
                table: "Links",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Links_Tests_TestId",
                table: "Links",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Links_Tests_TestId",
                table: "Links");

            migrationBuilder.DropIndex(
                name: "IX_Links_TestId",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "Links");

            migrationBuilder.AddColumn<int>(
                name: "LinkId",
                table: "Tests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_LinkId",
                table: "Tests",
                column: "LinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Links_LinkId",
                table: "Tests",
                column: "LinkId",
                principalTable: "Links",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
