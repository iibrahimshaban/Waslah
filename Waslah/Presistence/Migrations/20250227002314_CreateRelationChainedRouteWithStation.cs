using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waslah.Presistence.migrations
{
    /// <inheritdoc />
    public partial class CreateRelationChainedRouteWithStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteConnectors_ChainedRoutes_ChainedRouteID",
                table: "RouteConnectors");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteConnectors_Routes_MyRouteId",
                table: "RouteConnectors");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Stations_PrimaryLocId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Stations_SecondaryLocId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_StationTypes_Name",
                table: "StationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Routes",
                table: "Routes");

            migrationBuilder.RenameTable(
                name: "Routes",
                newName: "MyRoutes");

            migrationBuilder.RenameIndex(
                name: "IX_Routes_SecondaryLocId",
                table: "MyRoutes",
                newName: "IX_MyRoutes_SecondaryLocId");

            migrationBuilder.RenameIndex(
                name: "IX_Routes_PrimaryLocId",
                table: "MyRoutes",
                newName: "IX_MyRoutes_PrimaryLocId");

            migrationBuilder.AlterColumn<int>(
                name: "StationNo",
                table: "Stations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ModelId",
                table: "Stations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActivated",
                table: "Stations",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "GovNo",
                table: "Stations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CityNo",
                table: "Stations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Time",
                table: "ChainedRoutes",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "ChainedRoutes",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Distance",
                table: "ChainedRoutes",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Time",
                table: "MyRoutes",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "MyRoutes",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Distance",
                table: "MyRoutes",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Classification",
                table: "MyRoutes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyRoutes",
                table: "MyRoutes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ChainedRoutes_FirstLocId_LastLocId_NumberOfRides",
                table: "ChainedRoutes",
                columns: new[] { "FirstLocId", "LastLocId", "NumberOfRides" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChainedRoutes_LastLocId",
                table: "ChainedRoutes",
                column: "LastLocId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChainedRoutes_Stations_FirstLocId",
                table: "ChainedRoutes",
                column: "FirstLocId",
                principalTable: "Stations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChainedRoutes_Stations_LastLocId",
                table: "ChainedRoutes",
                column: "LastLocId",
                principalTable: "Stations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MyRoutes_Stations_PrimaryLocId",
                table: "MyRoutes",
                column: "PrimaryLocId",
                principalTable: "Stations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MyRoutes_Stations_SecondaryLocId",
                table: "MyRoutes",
                column: "SecondaryLocId",
                principalTable: "Stations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteConnectors_ChainedRoutes_ChainedRouteID",
                table: "RouteConnectors",
                column: "ChainedRouteID",
                principalTable: "ChainedRoutes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteConnectors_MyRoutes_MyRouteId",
                table: "RouteConnectors",
                column: "MyRouteId",
                principalTable: "MyRoutes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChainedRoutes_Stations_FirstLocId",
                table: "ChainedRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_ChainedRoutes_Stations_LastLocId",
                table: "ChainedRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_MyRoutes_Stations_PrimaryLocId",
                table: "MyRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_MyRoutes_Stations_SecondaryLocId",
                table: "MyRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteConnectors_ChainedRoutes_ChainedRouteID",
                table: "RouteConnectors");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteConnectors_MyRoutes_MyRouteId",
                table: "RouteConnectors");

            migrationBuilder.DropIndex(
                name: "IX_ChainedRoutes_FirstLocId_LastLocId_NumberOfRides",
                table: "ChainedRoutes");

            migrationBuilder.DropIndex(
                name: "IX_ChainedRoutes_LastLocId",
                table: "ChainedRoutes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MyRoutes",
                table: "MyRoutes");

            migrationBuilder.RenameTable(
                name: "MyRoutes",
                newName: "Routes");

            migrationBuilder.RenameIndex(
                name: "IX_MyRoutes_SecondaryLocId",
                table: "Routes",
                newName: "IX_Routes_SecondaryLocId");

            migrationBuilder.RenameIndex(
                name: "IX_MyRoutes_PrimaryLocId",
                table: "Routes",
                newName: "IX_Routes_PrimaryLocId");

            migrationBuilder.AlterColumn<int>(
                name: "StationNo",
                table: "Stations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModelId",
                table: "Stations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActivated",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GovNo",
                table: "Stations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CityNo",
                table: "Stations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Time",
                table: "ChainedRoutes",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "ChainedRoutes",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Distance",
                table: "ChainedRoutes",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Time",
                table: "Routes",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Routes",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Distance",
                table: "Routes",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Classification",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Routes",
                table: "Routes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StationTypes_Name",
                table: "StationTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteConnectors_ChainedRoutes_ChainedRouteID",
                table: "RouteConnectors",
                column: "ChainedRouteID",
                principalTable: "ChainedRoutes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteConnectors_Routes_MyRouteId",
                table: "RouteConnectors",
                column: "MyRouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Stations_PrimaryLocId",
                table: "Routes",
                column: "PrimaryLocId",
                principalTable: "Stations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Stations_SecondaryLocId",
                table: "Routes",
                column: "SecondaryLocId",
                principalTable: "Stations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
