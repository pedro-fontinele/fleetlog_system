using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AdjustColunmsReturnInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnInvoiceItems_ReturnInvoiceItems_ReturnInvoiceItemId",
                table: "ReturnInvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_ReturnInvoiceItems_ReturnInvoiceItemId",
                table: "ReturnInvoiceItems");

            migrationBuilder.DropColumn(
                name: "ReturnInvoiceItemId",
                table: "ReturnInvoiceItems");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceNumber",
                table: "ReturnInvoices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkDanfe",
                table: "ReturnInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkPdf",
                table: "ReturnInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Xml",
                table: "ReturnInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnInvoiceOrders_ExpeditionOrderId",
                table: "ReturnInvoiceOrders",
                column: "ExpeditionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnInvoiceOrders_ProductId",
                table: "ReturnInvoiceOrders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnInvoiceOrders_ReturnInvoiceId",
                table: "ReturnInvoiceOrders",
                column: "ReturnInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnInvoiceItems_ProductId",
                table: "ReturnInvoiceItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnInvoiceItems_Products_ProductId",
                table: "ReturnInvoiceItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnInvoiceOrders_ExpeditionOrders_ExpeditionOrderId",
                table: "ReturnInvoiceOrders",
                column: "ExpeditionOrderId",
                principalTable: "ExpeditionOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnInvoiceOrders_Products_ProductId",
                table: "ReturnInvoiceOrders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnInvoiceOrders_ReturnInvoices_ReturnInvoiceId",
                table: "ReturnInvoiceOrders",
                column: "ReturnInvoiceId",
                principalTable: "ReturnInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnInvoiceItems_Products_ProductId",
                table: "ReturnInvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnInvoiceOrders_ExpeditionOrders_ExpeditionOrderId",
                table: "ReturnInvoiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnInvoiceOrders_Products_ProductId",
                table: "ReturnInvoiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnInvoiceOrders_ReturnInvoices_ReturnInvoiceId",
                table: "ReturnInvoiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ReturnInvoiceOrders_ExpeditionOrderId",
                table: "ReturnInvoiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ReturnInvoiceOrders_ProductId",
                table: "ReturnInvoiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ReturnInvoiceOrders_ReturnInvoiceId",
                table: "ReturnInvoiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_ReturnInvoiceItems_ProductId",
                table: "ReturnInvoiceItems");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "ReturnInvoices");

            migrationBuilder.DropColumn(
                name: "LinkDanfe",
                table: "ReturnInvoices");

            migrationBuilder.DropColumn(
                name: "LinkPdf",
                table: "ReturnInvoices");

            migrationBuilder.DropColumn(
                name: "Xml",
                table: "ReturnInvoices");

            migrationBuilder.AddColumn<int>(
                name: "ReturnInvoiceItemId",
                table: "ReturnInvoiceItems",
                type: "int",
                nullable: true);

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
    }
}
