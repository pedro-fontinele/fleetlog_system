using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class CreatingOrderRequiredInformations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileFormat",
                table: "ExpeditionOrderTagShipping",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceNumber",
                table: "ExpeditionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceSerie",
                table: "ExpeditionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileFormat",
                table: "ExpeditionOrderTagShipping");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "ExpeditionOrders");

            migrationBuilder.DropColumn(
                name: "InvoiceSerie",
                table: "ExpeditionOrders");
        }
    }
}
