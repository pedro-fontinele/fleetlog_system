using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddedAddressingModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressingStreets",
                columns: table => new
                {
                    AddressingStreetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressingStreets", x => x.AddressingStreetID);
                });

            migrationBuilder.CreateTable(
                name: "AddressingPositions",
                columns: table => new
                {
                    AddressingPositionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressingStreetID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressingPositions", x => x.AddressingPositionID);
                    table.ForeignKey(
                        name: "FK_AddressingPositions_AddressingStreets_AddressingStreetID",
                        column: x => x.AddressingStreetID,
                        principalTable: "AddressingStreets",
                        principalColumn: "AddressingStreetID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressingPositions_AddressingStreetID",
                table: "AddressingPositions",
                column: "AddressingStreetID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressingPositions");

            migrationBuilder.DropTable(
                name: "AddressingStreets");
        }
    }
}
