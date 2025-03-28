 using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddingNewFieldsInReturnInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiptNoteId",
                table: "ReturnInvoiceItems",
                newName: "ReceiptNoteItemId");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceAccessKey",
                table: "ReturnInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rejection",
                table: "ReturnInvoices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceAccessKey",
                table: "ReturnInvoices");

            migrationBuilder.DropColumn(
                name: "Rejection",
                table: "ReturnInvoices");

            migrationBuilder.RenameColumn(
                name: "ReceiptNoteItemId",
                table: "ReturnInvoiceItems",
                newName: "ReceiptNoteId");
        }
    }
}
