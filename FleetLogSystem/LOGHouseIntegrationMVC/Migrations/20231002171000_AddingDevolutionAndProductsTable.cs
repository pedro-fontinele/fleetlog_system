using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddingDevolutionAndProductsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Devolutions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DevolutionAndProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    DevolutionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevolutionAndProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevolutionAndProduct_Devolutions_DevolutionId",
                        column: x => x.DevolutionId,
                        principalTable: "Devolutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevolutionAndProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DevolutionAndProduct_DevolutionId",
                table: "DevolutionAndProduct",
                column: "DevolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_DevolutionAndProduct_ProductId",
                table: "DevolutionAndProduct",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevolutionAndProduct");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Devolutions");
        }
    }
}
