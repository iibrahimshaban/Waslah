using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waslah.Presistence.migrations
{
    /// <inheritdoc />
    public partial class CreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChainedRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstLocId = table.Column<int>(type: "int", nullable: false),
                    LastLocId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Time = table.Column<double>(type: "float", nullable: false),
                    NumberOfRides = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChainedRoutes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationTypes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationTypes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    ModelId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Government = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    CityNo = table.Column<int>(type: "int", nullable: false),
                    StationNo = table.Column<int>(type: "int", nullable: false),
                    GovNo = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                    table.UniqueConstraint("AK_Stations_LocationId", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Stations_StationTypes_Type",
                        column: x => x.Type,
                        principalTable: "StationTypes",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrimaryLocId = table.Column<int>(type: "int", nullable: false),
                    SecondaryLocId = table.Column<int>(type: "int", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    Time = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Classification = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_Stations_PrimaryLocId",
                        column: x => x.PrimaryLocId,
                        principalTable: "Stations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Routes_Stations_SecondaryLocId",
                        column: x => x.SecondaryLocId,
                        principalTable: "Stations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "RouteConnectors",
                columns: table => new
                {
                    MyRouteId = table.Column<int>(type: "int", nullable: false),
                    ChainedRouteID = table.Column<int>(type: "int", nullable: false),
                    RouteOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteConnectors", x => new { x.MyRouteId, x.ChainedRouteID });
                    table.ForeignKey(
                        name: "FK_RouteConnectors_ChainedRoutes_ChainedRouteID",
                        column: x => x.ChainedRouteID,
                        principalTable: "ChainedRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteConnectors_Routes_MyRouteId",
                        column: x => x.MyRouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteConnectors_ChainedRouteID",
                table: "RouteConnectors",
                column: "ChainedRouteID");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_PrimaryLocId",
                table: "Routes",
                column: "PrimaryLocId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_SecondaryLocId",
                table: "Routes",
                column: "SecondaryLocId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_Type",
                table: "Stations",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_StationTypes_Name",
                table: "StationTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteConnectors");

            migrationBuilder.DropTable(
                name: "ChainedRoutes");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "StationTypes");
        }
    }
}
