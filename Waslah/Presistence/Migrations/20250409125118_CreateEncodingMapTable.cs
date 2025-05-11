using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waslah.Presistence.migrations
{
    /// <inheritdoc />
    public partial class CreateEncodingMapTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EncodingMaps",
                columns: table => new
                {
                    LocId = table.Column<int>(type: "int", nullable: false),
                    GovName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GovNumber = table.Column<int>(type: "int", nullable: false),
                    CityNumber = table.Column<int>(type: "int", nullable: false),
                    StationNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncodingMaps", x => x.LocId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncodingMaps");
        }
    }
}
