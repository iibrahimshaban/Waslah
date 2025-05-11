using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waslah.Presistence.migrations
{
    /// <inheritdoc />
    public partial class CreateTableAgencyTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgenciesTrip",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgencyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PickUpCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DestinationCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    NumberOfDays = table.Column<double>(type: "float", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PickUpDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgenciesTrip", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Link",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AgencyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Link", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Link_AgenciesTrip_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "AgenciesTrip",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgenciesTrip_AgencyName_PickUpCity_DestinationCity_PickUpDate",
                table: "AgenciesTrip",
                columns: new[] { "AgencyName", "PickUpCity", "DestinationCity", "PickUpDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Link_AgencyId",
                table: "Link",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Link_Name_Url",
                table: "Link",
                columns: new[] { "Name", "Url" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Link");

            migrationBuilder.DropTable(
                name: "AgenciesTrip");
        }
    }
}
