using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AdjustingPickingList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickingLists_ExpeditionOrders_ExpeditionOrderId",
                table: "PickingLists");

            migrationBuilder.DropIndex(
                name: "IX_PickingLists_ExpeditionOrderId",
                table: "PickingLists");

            migrationBuilder.DropColumn(
                name: "ExpeditionOrderId",
                table: "PickingLists");

            migrationBuilder.AddColumn<int>(
                name: "PickingListId",
                table: "ExpeditionOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionOrders_PickingListId",
                table: "ExpeditionOrders",
                column: "PickingListId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpeditionOrders_PickingLists_PickingListId",
                table: "ExpeditionOrders",
                column: "PickingListId",
                principalTable: "PickingLists",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpeditionOrders_PickingLists_PickingListId",
                table: "ExpeditionOrders");

            migrationBuilder.DropIndex(
                name: "IX_ExpeditionOrders_PickingListId",
                table: "ExpeditionOrders");

            migrationBuilder.DropColumn(
                name: "PickingListId",
                table: "ExpeditionOrders");

            migrationBuilder.AddColumn<int>(
                name: "ExpeditionOrderId",
                table: "PickingLists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_ExpeditionOrderId",
                table: "PickingLists",
                column: "ExpeditionOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PickingLists_ExpeditionOrders_ExpeditionOrderId",
                table: "PickingLists",
                column: "ExpeditionOrderId",
                principalTable: "ExpeditionOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
