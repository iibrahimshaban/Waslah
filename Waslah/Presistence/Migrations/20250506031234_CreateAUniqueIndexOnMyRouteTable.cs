using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waslah.Presistence.migrations
{
    /// <inheritdoc />
    public partial class CreateAUniqueIndexOnMyRouteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MyRoutes_PrimaryLocId",
                table: "MyRoutes");

            migrationBuilder.CreateIndex(
                name: "IX_MyRoutes_PrimaryLocId_SecondaryLocId",
                table: "MyRoutes",
                columns: new[] { "PrimaryLocId", "SecondaryLocId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MyRoutes_PrimaryLocId_SecondaryLocId",
                table: "MyRoutes");

            migrationBuilder.CreateIndex(
                name: "IX_MyRoutes_PrimaryLocId",
                table: "MyRoutes",
                column: "PrimaryLocId");
        }
    }
}
