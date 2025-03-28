using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class RemovingClientIdPickingList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickingLists_Clients_ClientId",
                table: "PickingLists");

            migrationBuilder.DropIndex(
                name: "IX_PickingLists_ClientId",
                table: "PickingLists");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "PickingLists");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "ExpeditionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionOrders_ClientId",
                table: "ExpeditionOrders",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpeditionOrders_Clients_ClientId",
                table: "ExpeditionOrders",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpeditionOrders_Clients_ClientId",
                table: "ExpeditionOrders");

            migrationBuilder.DropIndex(
                name: "IX_ExpeditionOrders_ClientId",
                table: "ExpeditionOrders");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "PickingLists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "ExpeditionOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_ClientId",
                table: "PickingLists",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PickingLists_Clients_ClientId",
                table: "PickingLists",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
