using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class DeletingClientParamters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrations_Clients_ClientId",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "BlingApiKey",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TinyApiKey",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Integrations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_Clients_ClientId",
                table: "Integrations",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrations_Clients_ClientId",
                table: "Integrations");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "Integrations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlingApiKey",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TinyApiKey",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_Clients_ClientId",
                table: "Integrations",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
