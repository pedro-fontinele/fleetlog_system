using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddPickingListAndPickListItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalState",
                table: "ExpeditionOrders");

            migrationBuilder.CreateTable(
                name: "PickingLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Responsible = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ExpeditionOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickingLists_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickingLists_ExpeditionOrders_ExpeditionOrderId",
                        column: x => x.ExpeditionOrderId,
                        principalTable: "ExpeditionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickingListItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickingListId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    QuantityInspection = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickingListItems_PickingLists_PickingListId",
                        column: x => x.PickingListId,
                        principalTable: "PickingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickingListItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpeditionOrderTagShipping_ExpeditionOrderId",
                table: "ExpeditionOrderTagShipping",
                column: "ExpeditionOrderId",
                unique: true,
                filter: "[ExpeditionOrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PickingListItems_PickingListId",
                table: "PickingListItems",
                column: "PickingListId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingListItems_ProductId",
                table: "PickingListItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_ClientId",
                table: "PickingLists",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_ExpeditionOrderId",
                table: "PickingLists",
                column: "ExpeditionOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpeditionOrderTagShipping_ExpeditionOrders_ExpeditionOrderId",
                table: "ExpeditionOrderTagShipping",
                column: "ExpeditionOrderId",
                principalTable: "ExpeditionOrders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpeditionOrderTagShipping_ExpeditionOrders_ExpeditionOrderId",
                table: "ExpeditionOrderTagShipping");

            migrationBuilder.DropTable(
                name: "PickingListItems");

            migrationBuilder.DropTable(
                name: "PickingLists");

            migrationBuilder.DropIndex(
                name: "IX_ExpeditionOrderTagShipping_ExpeditionOrderId",
                table: "ExpeditionOrderTagShipping");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalState",
                table: "ExpeditionOrders",
                type: "int",
                nullable: true);
        }
    }
}
