using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddNewFieldsInReceiptNoteModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EntryDate",
                table: "ReceiptNotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                table: "ReceiptNotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptNotes_ClientId",
                table: "ReceiptNotes",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptNotes_Clients_ClientId",
                table: "ReceiptNotes",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptNotes_Clients_ClientId",
                table: "ReceiptNotes");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptNotes_ClientId",
                table: "ReceiptNotes");

            migrationBuilder.DropColumn(
                name: "EntryDate",
                table: "ReceiptNotes");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "ReceiptNotes");
        }
    }
}
