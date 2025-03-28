using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddedLotInformations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lote",
                table: "ReceiptNoteItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Validade",
                table: "ReceiptNoteItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lote",
                table: "InvoiceItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Validade",
                table: "InvoiceItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lote",
                table: "ReceiptNoteItems");

            migrationBuilder.DropColumn(
                name: "Validade",
                table: "ReceiptNoteItems");

            migrationBuilder.DropColumn(
                name: "Lote",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "Validade",
                table: "InvoiceItems");
        }
    }
}
