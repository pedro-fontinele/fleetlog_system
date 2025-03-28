using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class CreateTableRetryQueue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RetryQueues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HookInputId = table.Column<int>(type: "int", nullable: false),
                    LastTry = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tries = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetryQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetryQueues_HookInputs_HookInputId",
                        column: x => x.HookInputId,
                        principalTable: "HookInputs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RetryQueues_HookInputId",
                table: "RetryQueues",
                column: "HookInputId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetryQueues");
        }
    }
}
