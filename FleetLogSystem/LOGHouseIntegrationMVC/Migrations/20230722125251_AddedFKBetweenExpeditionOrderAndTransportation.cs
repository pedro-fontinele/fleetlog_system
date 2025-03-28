using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddedFKBetweenExpeditionOrderAndTransportation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PackingListTransportationId",
                table: "ExpeditionOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VolumeQuantity",
                table: "ExpeditionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionOrders_PackingListTransportationId",
                table: "ExpeditionOrders",
                column: "PackingListTransportationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpeditionOrders_PackingListTransportations_PackingListTransportationId",
                table: "ExpeditionOrders",
                column: "PackingListTransportationId",
                principalTable: "PackingListTransportations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpeditionOrders_PackingListTransportations_PackingListTransportationId",
                table: "ExpeditionOrders");

            migrationBuilder.DropIndex(
                name: "IX_ExpeditionOrders_PackingListTransportationId",
                table: "ExpeditionOrders");

            migrationBuilder.DropColumn(
                name: "PackingListTransportationId",
                table: "ExpeditionOrders");

            migrationBuilder.DropColumn(
                name: "VolumeQuantity",
                table: "ExpeditionOrders");
        }
    }
}
