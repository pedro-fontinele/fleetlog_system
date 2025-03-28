using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class ChanginExternalNumberItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ERPId",
                table: "ExpeditionOrderItems");

            migrationBuilder.AddColumn<string>(
                name: "ExternalNumberItem",
                table: "ExpeditionOrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalNumberItem",
                table: "ExpeditionOrderItems");

            migrationBuilder.AddColumn<string>(
                name: "ERPId",
                table: "ExpeditionOrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
