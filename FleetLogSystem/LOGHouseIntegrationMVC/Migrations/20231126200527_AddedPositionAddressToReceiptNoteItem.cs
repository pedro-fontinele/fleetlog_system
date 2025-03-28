using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddedPositionAddressToReceiptNoteItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PositionAddress",
                table: "ReceiptNoteItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PositionAddress",
                table: "InvoiceItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionAddress",
                table: "ReceiptNoteItems");

            migrationBuilder.DropColumn(
                name: "PositionAddress",
                table: "InvoiceItems");
        }
    }
}
