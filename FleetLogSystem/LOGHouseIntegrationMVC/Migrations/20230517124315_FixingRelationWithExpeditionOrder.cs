using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class FixingRelationWithExpeditionOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpeditionOrderItems_ShippingDetails_ShippingDetailsId",
                table: "ExpeditionOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_ExpeditionOrderItems_ShippingDetailsId",
                table: "ExpeditionOrderItems");

            migrationBuilder.DropColumn(
                name: "ShippingDetailsId",
                table: "ExpeditionOrderItems");

            migrationBuilder.AddColumn<int>(
                name: "ShippingDetailsId",
                table: "ExpeditionOrders",
                type: "int",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpeditionOrders_ShippingDetails_ShippingDetailsId1",
                table: "ExpeditionOrders");

            migrationBuilder.DropIndex(
                name: "IX_ExpeditionOrders_ShippingDetailsId1",
                table: "ExpeditionOrders");

            migrationBuilder.DropColumn(
                name: "ShippingDetailsId",
                table: "ExpeditionOrders");

            migrationBuilder.DropColumn(
                name: "ShippingDetailsId1",
                table: "ExpeditionOrders");

            migrationBuilder.AddColumn<int>(
                name: "ShippingDetailsId",
                table: "ExpeditionOrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionOrderItems_ShippingDetailsId",
                table: "ExpeditionOrderItems",
                column: "ShippingDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpeditionOrderItems_ShippingDetails_ShippingDetailsId",
                table: "ExpeditionOrderItems",
                column: "ShippingDetailsId",
                principalTable: "ShippingDetails",
                principalColumn: "Id");
        }
    }
}
