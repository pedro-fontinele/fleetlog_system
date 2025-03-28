using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class removeHIFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RetryQueues_HookInputs_HookInputId",
                table: "RetryQueues");

            migrationBuilder.DropIndex(
                name: "IX_RetryQueues_HookInputId",
                table: "RetryQueues");

            migrationBuilder.AlterColumn<int>(
                name: "HookInputId",
                table: "RetryQueues",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "HookInputId",
                table: "RetryQueues",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RetryQueues_HookInputId",
                table: "RetryQueues",
                column: "HookInputId");

            migrationBuilder.AddForeignKey(
                name: "FK_RetryQueues_HookInputs_HookInputId",
                table: "RetryQueues",
                column: "HookInputId",
                principalTable: "HookInputs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
