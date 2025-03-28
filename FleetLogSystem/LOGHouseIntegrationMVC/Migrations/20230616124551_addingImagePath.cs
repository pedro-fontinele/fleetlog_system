using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class addingImagePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packings_Clients_ClientId",
                table: "Packings");

            migrationBuilder.DropForeignKey(
                name: "FK_Packings_ExpeditionOrders_ExpeditionOrderId",
                table: "Packings");

            migrationBuilder.AlterColumn<int>(
                name: "ExpeditionOrderId",
                table: "Packings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Packings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Packings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Packings_Clients_ClientId",
                table: "Packings",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Packings_ExpeditionOrders_ExpeditionOrderId",
                table: "Packings",
                column: "ExpeditionOrderId",
                principalTable: "ExpeditionOrders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packings_Clients_ClientId",
                table: "Packings");

            migrationBuilder.DropForeignKey(
                name: "FK_Packings_ExpeditionOrders_ExpeditionOrderId",
                table: "Packings");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Packings");

            migrationBuilder.AlterColumn<int>(
                name: "ExpeditionOrderId",
                table: "Packings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Packings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Packings_Clients_ClientId",
                table: "Packings",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Packings_ExpeditionOrders_ExpeditionOrderId",
                table: "Packings",
                column: "ExpeditionOrderId",
                principalTable: "ExpeditionOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
