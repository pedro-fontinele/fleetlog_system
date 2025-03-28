using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddNewInventoryMovimentsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Origin",
                table: "InventoryMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockSlotMoviment",
                table: "InventoryMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Origin",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "StockSlotMoviment",
                table: "InventoryMovements");
        }
    }
}
