using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class ReceiptNoteChangeLogGenerated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LotGenerated",
                table: "ReceiptNotes");

            migrationBuilder.AddColumn<int>(
                name: "LotGenerated",
                table: "ReceiptNoteItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LotGenerated",
                table: "ReceiptNoteItems");

            migrationBuilder.AddColumn<int>(
                name: "LotGenerated",
                table: "ReceiptNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
