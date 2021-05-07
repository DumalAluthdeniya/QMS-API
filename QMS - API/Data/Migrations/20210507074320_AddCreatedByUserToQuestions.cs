﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace QMS_API.Data.Migrations
{
    public partial class AddCreatedByUserToQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Questions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CreatedById",
                table: "Questions",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_CreatedById",
                table: "Questions",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AspNetUsers_CreatedById",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CreatedById",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Questions");
        }
    }
}
