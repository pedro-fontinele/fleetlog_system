using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class TableDevolutionAndProudct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DevolutionAndProduct_Devolutions_DevolutionId",
                table: "DevolutionAndProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_DevolutionAndProduct_Products_ProductId",
                table: "DevolutionAndProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DevolutionAndProduct",
                table: "DevolutionAndProduct");

            migrationBuilder.RenameTable(
                name: "DevolutionAndProduct",
                newName: "DevolutionAndProducts");

            migrationBuilder.RenameIndex(
                name: "IX_DevolutionAndProduct_ProductId",
                table: "DevolutionAndProducts",
                newName: "IX_DevolutionAndProducts_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_DevolutionAndProduct_DevolutionId",
                table: "DevolutionAndProducts",
                newName: "IX_DevolutionAndProducts_DevolutionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DevolutionAndProducts",
                table: "DevolutionAndProducts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DevolutionAndProducts_Devolutions_DevolutionId",
                table: "DevolutionAndProducts",
                column: "DevolutionId",
                principalTable: "Devolutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DevolutionAndProducts_Products_ProductId",
                table: "DevolutionAndProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DevolutionAndProducts_Devolutions_DevolutionId",
                table: "DevolutionAndProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_DevolutionAndProducts_Products_ProductId",
                table: "DevolutionAndProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DevolutionAndProducts",
                table: "DevolutionAndProducts");

            migrationBuilder.RenameTable(
                name: "DevolutionAndProducts",
                newName: "DevolutionAndProduct");

            migrationBuilder.RenameIndex(
                name: "IX_DevolutionAndProducts_ProductId",
                table: "DevolutionAndProduct",
                newName: "IX_DevolutionAndProduct_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_DevolutionAndProducts_DevolutionId",
                table: "DevolutionAndProduct",
                newName: "IX_DevolutionAndProduct_DevolutionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DevolutionAndProduct",
                table: "DevolutionAndProduct",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DevolutionAndProduct_Devolutions_DevolutionId",
                table: "DevolutionAndProduct",
                column: "DevolutionId",
                principalTable: "Devolutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DevolutionAndProduct_Products_ProductId",
                table: "DevolutionAndProduct",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
