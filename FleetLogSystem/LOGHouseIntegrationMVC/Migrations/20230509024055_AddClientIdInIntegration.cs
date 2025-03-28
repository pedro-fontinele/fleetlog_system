using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddClientIdInIntegration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Integrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_ClientId",
                table: "Integrations",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_Clients_ClientId",
                table: "Integrations",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrations_Clients_ClientId",
                table: "Integrations");

            migrationBuilder.DropIndex(
                name: "IX_Integrations_ClientId",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Integrations");
        }
    }
}
