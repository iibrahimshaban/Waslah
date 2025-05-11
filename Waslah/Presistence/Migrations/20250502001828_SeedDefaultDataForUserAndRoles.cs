using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Waslah.Presistence.migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultDataForUserAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDisabled", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "01968e52-bf6e-781f-a030-df2af8539b2d", "01968e52-bf6e-781f-a030-df2b49eabe1c", false, false, "Admin", "ADMIN" },
                    { "01968e52-bf6e-781f-a030-df2c4fdc74fd", "01968e52-bf6e-781f-a030-df2d340142a4", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsDisabled", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePhotoPath", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "01968e52-bf6e-781f-a030-df270bd865be", 0, "01968e52-bf6e-781f-a030-df28ddfd3090", "IibrahimKhaled@gmail.com", true, "Ibrahim", false, "Shaban", true, null, "IIBRAHIMKHALED@GMAIL.COM", "IBRAHIM", "AQAAAAIAAYagAAAAELHGazbET00Upjxl0rBA4nwtpT2aX6c1/Akz8H23+wLysfHdFIZlBtNVfFtCBexAKQ==", null, false, "", "01968e52bf6e781fa030df29ab177dea", false, "Ibrahim" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "Permissions", "User:Read", "01968e52-bf6e-781f-a030-df2af8539b2d" },
                    { 2, "Permissions", "User:Create", "01968e52-bf6e-781f-a030-df2af8539b2d" },
                    { 3, "Permissions", "User:Update", "01968e52-bf6e-781f-a030-df2af8539b2d" },
                    { 4, "Permissions", "Roles:Read", "01968e52-bf6e-781f-a030-df2af8539b2d" },
                    { 5, "Permissions", "Roles:Create", "01968e52-bf6e-781f-a030-df2af8539b2d" },
                    { 6, "Permissions", "Roles:Update", "01968e52-bf6e-781f-a030-df2af8539b2d" },
                    { 7, "Permissions", "Results:Read", "01968e52-bf6e-781f-a030-df2af8539b2d" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "01968e52-bf6e-781f-a030-df2af8539b2d", "01968e52-bf6e-781f-a030-df270bd865be" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "01968e52-bf6e-781f-a030-df2c4fdc74fd");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "01968e52-bf6e-781f-a030-df2af8539b2d", "01968e52-bf6e-781f-a030-df270bd865be" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "01968e52-bf6e-781f-a030-df2af8539b2d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "01968e52-bf6e-781f-a030-df270bd865be");
        }
    }
}
