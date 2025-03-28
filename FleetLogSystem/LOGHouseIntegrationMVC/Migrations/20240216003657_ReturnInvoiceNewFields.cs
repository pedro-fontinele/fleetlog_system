using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class ReturnInvoiceNewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnInvoiceId",
                table: "ExpeditionOrders");

            migrationBuilder.AddColumn<int>(
                name: "ReturnInvoiceItemId",
                table: "ReturnInvoiceItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LotGenerated",
                table: "ReceiptNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnedInvoiceGenerated",
                table: "ExpeditionOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExpeditionOrdersLotNotFounded",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpeditionOrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpeditionOrdersLotNotFounded", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReturnInvoiceOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnInvoiceId = table.Column<int>(type: "int", nullable: false),
                    ExpeditionOrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnInvoiceOrders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnInvoiceItems_ReturnInvoiceItemId",
                table: "ReturnInvoiceItems",
                column: "ReturnInvoiceItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnInvoiceItems_ReturnInvoiceItems_ReturnInvoiceItemId",
                table: "ReturnInvoiceItems",
                column: "ReturnInvoiceItemId",
                principalTable: "ReturnInvoiceItems",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnInvoiceItems_ReturnInvoiceItems_ReturnInvoiceItemId",
                table: "ReturnInvoiceItems");

            migrationBuilder.DropTable(
                name: "ExpeditionOrdersLotNotFounded");

            migrationBuilder.DropTable(
                name: "ReturnInvoiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ReturnInvoiceItems_ReturnInvoiceItemId",
                table: "ReturnInvoiceItems");

            migrationBuilder.DropColumn(
                name: "ReturnInvoiceItemId",
                table: "ReturnInvoiceItems");

            migrationBuilder.DropColumn(
                name: "LotGenerated",
                table: "ReceiptNotes");

            migrationBuilder.DropColumn(
                name: "ReturnedInvoiceGenerated",
                table: "ExpeditionOrders");

            migrationBuilder.AddColumn<int>(
                name: "ReturnInvoiceId",
                table: "ExpeditionOrders",
                type: "int",
                nullable: true);
        }
    }
}
