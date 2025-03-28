using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class PackingListTransportationHistoryCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PackingListTransportationId",
                table: "Packings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PackingListTransportationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PackingListTransportationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingListTransportationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackingListTransportationHistories_PackingListTransportations_PackingListTransportationId",
                        column: x => x.PackingListTransportationId,
                        principalTable: "PackingListTransportations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PackingListTransportationHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Packings_PackingListTransportationId",
                table: "Packings",
                column: "PackingListTransportationId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingListTransportationHistories_PackingListTransportationId",
                table: "PackingListTransportationHistories",
                column: "PackingListTransportationId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingListTransportationHistories_UserId",
                table: "PackingListTransportationHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packings_PackingListTransportations_PackingListTransportationId",
                table: "Packings",
                column: "PackingListTransportationId",
                principalTable: "PackingListTransportations",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packings_PackingListTransportations_PackingListTransportationId",
                table: "Packings");

            migrationBuilder.DropTable(
                name: "PackingListTransportationHistories");

            migrationBuilder.DropIndex(
                name: "IX_Packings_PackingListTransportationId",
                table: "Packings");

            migrationBuilder.DropColumn(
                name: "PackingListTransportationId",
                table: "Packings");
        }
    }
}
