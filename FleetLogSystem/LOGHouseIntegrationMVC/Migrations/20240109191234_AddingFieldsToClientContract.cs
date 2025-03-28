using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class AddingFieldsToClientContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExcessOrderValue",
                table: "ClientContracts",
                type: "decimal(18,2)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InsurancePercentage",
                table: "ClientContracts",
                type: "decimal(18,2)",
                nullable: true,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcessOrderValue",
                table: "ClientContracts");

            migrationBuilder.DropColumn(
                name: "InsurancePercentage",
                table: "ClientContracts");
        }
    }
}
