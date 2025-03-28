using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddingPriorityFieldInPickingListModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "PickingLists",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "PickingLists");
        }
    }
}
