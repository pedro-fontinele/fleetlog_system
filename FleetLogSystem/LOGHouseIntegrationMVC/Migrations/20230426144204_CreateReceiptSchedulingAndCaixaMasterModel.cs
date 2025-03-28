using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class CreateReceiptSchedulingAndCaixaMasterModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaixaMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ReceiptNoteItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaixaMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaixaMasters_ReceiptNoteItems_ReceiptNoteItemId",
                        column: x => x.ReceiptNoteItemId,
                        principalTable: "ReceiptNoteItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptSchedulings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NfNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VolumesQuantity = table.Column<int>(type: "int", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilesPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptSchedulings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptSchedulings_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaixaMasters_ReceiptNoteItemId",
                table: "CaixaMasters",
                column: "ReceiptNoteItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptSchedulings_ClientId",
                table: "ReceiptSchedulings",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaixaMasters");

            migrationBuilder.DropTable(
                name: "ReceiptSchedulings");
        }
    }
}
