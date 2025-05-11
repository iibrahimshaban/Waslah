using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waslah.Presistence.migrations
{
    /// <inheritdoc />
    public partial class AddAudiatbleEntityToStationAndMyRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Stations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Stations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Stations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Stations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MyRoutes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "MyRoutes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MyRoutes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "MyRoutes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stations_CreatedById",
                table: "Stations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_UpdatedById",
                table: "Stations",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MyRoutes_CreatedById",
                table: "MyRoutes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MyRoutes_UpdatedById",
                table: "MyRoutes",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_MyRoutes_AspNetUsers_CreatedById",
                table: "MyRoutes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MyRoutes_AspNetUsers_UpdatedById",
                table: "MyRoutes",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stations_AspNetUsers_CreatedById",
                table: "Stations",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stations_AspNetUsers_UpdatedById",
                table: "Stations",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyRoutes_AspNetUsers_CreatedById",
                table: "MyRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_MyRoutes_AspNetUsers_UpdatedById",
                table: "MyRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_Stations_AspNetUsers_CreatedById",
                table: "Stations");

            migrationBuilder.DropForeignKey(
                name: "FK_Stations_AspNetUsers_UpdatedById",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Stations_CreatedById",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Stations_UpdatedById",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_MyRoutes_CreatedById",
                table: "MyRoutes");

            migrationBuilder.DropIndex(
                name: "IX_MyRoutes_UpdatedById",
                table: "MyRoutes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MyRoutes");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "MyRoutes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MyRoutes");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "MyRoutes");
        }
    }
}
