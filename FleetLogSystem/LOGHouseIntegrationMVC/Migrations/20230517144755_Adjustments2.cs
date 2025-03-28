using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class Adjustments2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpeditionOrders_ShippingDetails_ShippingDetailsId1",
                table: "ExpeditionOrders");

            migrationBuilder.DropIndex(
                name: "IX_ExpeditionOrders_ShippingDetailsId1",
                table: "ExpeditionOrders");

            migrationBuilder.DropColumn(
                name: "ExpeditionOrderId",
                table: "ShippingDetails");

            migrationBuilder.DropColumn(
                name: "ShippingDetailsId1",
                table: "ExpeditionOrders");

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionOrders_ShippingDetailsId",
                table: "ExpeditionOrders",
                column: "ShippingDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpeditionOrders_ShippingDetails_ShippingDetailsId",
                table: "ExpeditionOrders",
                column: "ShippingDetailsId",
                principalTable: "ShippingDetails",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpeditionOrders_ShippingDetails_ShippingDetailsId",
                table: "ExpeditionOrders");

            migrationBuilder.DropIndex(
                name: "IX_ExpeditionOrders_ShippingDetailsId",
                table: "ExpeditionOrders");

            migrationBuilder.AddColumn<int>(
                name: "ExpeditionOrderId",
                table: "ShippingDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippingDetailsId1",
                table: "ExpeditionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionOrders_ShippingDetailsId1",
                table: "ExpeditionOrders",
                column: "ShippingDetailsId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpeditionOrders_ShippingDetails_ShippingDetailsId1",
                table: "ExpeditionOrders",
                column: "ShippingDetailsId1",
                principalTable: "ShippingDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
