using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class Renamingcolunmname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ReturnInvoiceItems_ReceiptNoteItemId",
                table: "ReturnInvoiceItems",
                column: "ReceiptNoteItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnInvoiceItems_ReceiptNoteItems_ReceiptNoteItemId",
                table: "ReturnInvoiceItems",
                column: "ReceiptNoteItemId",
                principalTable: "ReceiptNoteItems",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnInvoiceItems_ReceiptNoteItems_ReceiptNoteItemId",
                table: "ReturnInvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_ReturnInvoiceItems_ReceiptNoteItemId",
                table: "ReturnInvoiceItems");

        }
    }
}
