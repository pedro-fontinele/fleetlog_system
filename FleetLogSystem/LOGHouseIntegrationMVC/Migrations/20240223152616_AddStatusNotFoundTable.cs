using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddStatusNotFoundTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ExpeditionOrdersLotNotFounded",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionOrdersLotNotFounded_ExpeditionOrderId",
                table: "ExpeditionOrdersLotNotFounded",
                column: "ExpeditionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionOrdersLotNotFounded_ProductId",
                table: "ExpeditionOrdersLotNotFounded",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpeditionOrdersLotNotFounded_ExpeditionOrders_ExpeditionOrderId",
                table: "ExpeditionOrdersLotNotFounded",
                column: "ExpeditionOrderId",
                principalTable: "ExpeditionOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpeditionOrdersLotNotFounded_Products_ProductId",
                table: "ExpeditionOrdersLotNotFounded",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpeditionOrdersLotNotFounded_ExpeditionOrders_ExpeditionOrderId",
                table: "ExpeditionOrdersLotNotFounded");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpeditionOrdersLotNotFounded_Products_ProductId",
                table: "ExpeditionOrdersLotNotFounded");

            migrationBuilder.DropIndex(
                name: "IX_ExpeditionOrdersLotNotFounded_ExpeditionOrderId",
                table: "ExpeditionOrdersLotNotFounded");

            migrationBuilder.DropIndex(
                name: "IX_ExpeditionOrdersLotNotFounded_ProductId",
                table: "ExpeditionOrdersLotNotFounded");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ExpeditionOrdersLotNotFounded");
        }
    }
}
