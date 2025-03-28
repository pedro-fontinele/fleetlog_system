using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class CreatingReletionshipBetweenLabelsAndReceiptNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceiptNoteId",
                table: "LabelBillings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabelBillings_ReceiptNoteId",
                table: "LabelBillings",
                column: "ReceiptNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabelBillings_ReceiptNotes_ReceiptNoteId",
                table: "LabelBillings",
                column: "ReceiptNoteId",
                principalTable: "ReceiptNotes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabelBillings_ReceiptNotes_ReceiptNoteId",
                table: "LabelBillings");

            migrationBuilder.DropIndex(
                name: "IX_LabelBillings_ReceiptNoteId",
                table: "LabelBillings");

            migrationBuilder.DropColumn(
                name: "ReceiptNoteId",
                table: "LabelBillings");
        }
    }
}
