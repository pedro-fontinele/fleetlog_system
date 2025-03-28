using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOGHouseSystem.Migrations
{
    public partial class CreateTablePackingListTransportations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PackingListTransportations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransportationPersonId = table.Column<int>(type: "int", nullable: true),
                    ShippingCompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingListTransportations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackingListTransportations_ShippingCompanies_ShippingCompanyId",
                        column: x => x.ShippingCompanyId,
                        principalTable: "ShippingCompanies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PackingListTransportations_TransportationPeople_TransportationPersonId",
                        column: x => x.TransportationPersonId,
                        principalTable: "TransportationPeople",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackingListTransportations_ShippingCompanyId",
                table: "PackingListTransportations",
                column: "ShippingCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingListTransportations_TransportationPersonId",
                table: "PackingListTransportations",
                column: "TransportationPersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackingListTransportations");
        }
    }
}
