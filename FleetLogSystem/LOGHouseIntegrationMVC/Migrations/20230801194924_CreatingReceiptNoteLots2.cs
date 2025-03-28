using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class CreatingReceiptNoteLots2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiptNoteLots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    LastReceiptNoteId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InputQuantity = table.Column<double>(type: "float", nullable: false),
                    OutputQuantity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptNoteLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptNoteLots_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptNoteLots_ReceiptNotes_LastReceiptNoteId",
                        column: x => x.LastReceiptNoteId,
                        principalTable: "ReceiptNotes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptNoteLots_LastReceiptNoteId",
                table: "ReceiptNoteLots",
                column: "LastReceiptNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptNoteLots_ProductId",
                table: "ReceiptNoteLots",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptNoteLots");
        }
    }
}
