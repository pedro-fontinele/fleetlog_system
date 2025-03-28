using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddingStateRegistrationFiel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackingItems_Products_ProductId",
                table: "PackingItems");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "PackingItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateRegistration",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PackingItems_Products_ProductId",
                table: "PackingItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackingItems_Products_ProductId",
                table: "PackingItems");

            migrationBuilder.DropColumn(
                name: "StateRegistration",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "PackingItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PackingItems_Products_ProductId",
                table: "PackingItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
