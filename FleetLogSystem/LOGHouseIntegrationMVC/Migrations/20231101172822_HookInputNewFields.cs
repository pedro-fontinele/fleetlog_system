using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class HookInputNewFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "HookInputs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "HookInputs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "HookInputs",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "HookInputs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "HookInputs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "HookInputs");
        }
    }
}
