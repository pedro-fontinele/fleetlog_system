using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class CreateTablePostionAndProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PositionsAndProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    AddressingPositionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionsAndProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionsAndProducts_AddressingPositions_AddressingPositionId",
                        column: x => x.AddressingPositionId,
                        principalTable: "AddressingPositions",
                        principalColumn: "AddressingPositionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PositionsAndProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PositionsAndProducts_AddressingPositionId",
                table: "PositionsAndProducts",
                column: "AddressingPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionsAndProducts_ProductId",
                table: "PositionsAndProducts",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PositionsAndProducts");
        }
    }
}
