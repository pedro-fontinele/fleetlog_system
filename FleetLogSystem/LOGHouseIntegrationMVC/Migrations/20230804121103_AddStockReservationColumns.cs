using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddStockReservationColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SentEmails_Clients_ClientId",
                table: "SentEmails");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "SentEmails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "StockReservationQuantity",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_SentEmails_Clients_ClientId",
                table: "SentEmails",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SentEmails_Clients_ClientId",
                table: "SentEmails");

            migrationBuilder.DropColumn(
                name: "StockReservationQuantity",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "SentEmails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SentEmails_Clients_ClientId",
                table: "SentEmails",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
