using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddingDevolutionAndReceiptNoteModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DevolutionAndReceiptNote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptNoteId = table.Column<int>(type: "int", nullable: false),
                    DevolutionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevolutionAndReceiptNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevolutionAndReceiptNote_Devolutions_DevolutionId",
                        column: x => x.DevolutionId,
                        principalTable: "Devolutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevolutionAndReceiptNote_ReceiptNotes_ReceiptNoteId",
                        column: x => x.ReceiptNoteId,
                        principalTable: "ReceiptNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });



            migrationBuilder.CreateIndex(
                name: "IX_DevolutionAndReceiptNote_DevolutionId",
                table: "DevolutionAndReceiptNote",
                column: "DevolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_DevolutionAndReceiptNote_ReceiptNoteId",
                table: "DevolutionAndReceiptNote",
                column: "ReceiptNoteId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
