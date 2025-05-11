using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waslah.Presistence.migrations
{
    /// <inheritdoc />
    public partial class CreateTableAgnecyLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Link");

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AgencyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Links_AgenciesTrip_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "AgenciesTrip",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Links_AgencyId",
                table: "Links",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Links_Name_Url",
                table: "Links",
                columns: new[] { "Name", "Url" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.CreateTable(
                name: "Link",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AgencyId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                name: "IX_Link_AgencyId",
                table: "Link",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Link_Name_Url",
                table: "Link",
                columns: new[] { "Name", "Url" },
                unique: true);
        }
    }
}
