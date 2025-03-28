using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class changingReceiptNoteLots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptNoteLots_ReceiptNotes_LastReceiptNoteId",
                table: "ReceiptNoteLots");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptNoteLots_LastReceiptNoteId",
                table: "ReceiptNoteLots");

            migrationBuilder.DropColumn(
                name: "LastReceiptNoteId",
                table: "ReceiptNoteLots");

            migrationBuilder.AddColumn<int>(
                name: "ReceiptNoteId",
                table: "ReceiptNoteLots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptNoteLots_ReceiptNoteId",
                table: "ReceiptNoteLots",
                column: "ReceiptNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptNoteLots_ReceiptNotes_ReceiptNoteId",
                table: "ReceiptNoteLots",
                column: "ReceiptNoteId",
                principalTable: "ReceiptNotes",
                principalColumn: "Id"
                /*onDelete: ReferentialAction.Cascade*/);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptNoteLots_ReceiptNotes_ReceiptNoteId",
                table: "ReceiptNoteLots");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptNoteLots_ReceiptNoteId",
                table: "ReceiptNoteLots");

            migrationBuilder.DropColumn(
                name: "ReceiptNoteId",
                table: "ReceiptNoteLots");

            migrationBuilder.AddColumn<int>(
                name: "LastReceiptNoteId",
                table: "ReceiptNoteLots",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptNoteLots_LastReceiptNoteId",
                table: "ReceiptNoteLots",
                column: "LastReceiptNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptNoteLots_ReceiptNotes_LastReceiptNoteId",
                table: "ReceiptNoteLots",
                column: "LastReceiptNoteId",
                principalTable: "ReceiptNotes",
                principalColumn: "Id");
        }
    }
}
