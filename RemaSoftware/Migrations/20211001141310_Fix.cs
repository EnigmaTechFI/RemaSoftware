using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemaSoftware.Migrations
{
    public partial class Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5c1ee0a6-c87d-40b1-b50b-e0fe9c2f4756");

            migrationBuilder.AlterColumn<double>(
                name: "Price_Uni",
                table: "Warehouse_Stocks",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Price_Tot",
                table: "Warehouse_Stocks",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "Birthday", "Name", "Surname" },
                values: new object[] { "f3c7161e-abf9-4b34-acd6-7db13e4e4ec1", 0, "2a9c6b4d-b99d-4999-9cbd-03bfddd3268f", "MyUser", "lorenzo.vettori11@gmail.com", false, true, null, "LORENZO.VETTORI11@GMAIL.COM", "LORE_VETTO11", "AQAAAAEAACcQAAAAEAO6GoyqXpTgsJ3tC72YWwgM842MNb+CIFxLg2v6OFYwB+HN0Wcs3srD/SRyRrSPGg==", null, false, "a1ca1006-e0c5-4f11-bc40-866d219c5ca1", false, "lore_vetto11", new DateTime(1998, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lorenzo", "Vettori" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f3c7161e-abf9-4b34-acd6-7db13e4e4ec1");

            migrationBuilder.AlterColumn<float>(
                name: "Price_Uni",
                table: "Warehouse_Stocks",
                type: "real",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<float>(
                name: "Price_Tot",
                table: "Warehouse_Stocks",
                type: "real",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "Birthday", "Name", "Surname" },
                values: new object[] { "5c1ee0a6-c87d-40b1-b50b-e0fe9c2f4756", 0, "6ea729a6-971c-4fee-8393-7cd91c701048", "MyUser", "lorenzo.vettori11@gmail.com", false, true, null, "LORENZO.VETTORI11@GMAIL.COM", "LORE_VETTO11", "AQAAAAEAACcQAAAAEMgEBVMyNVi7ABHrz++Kb/bxe6zbZXJKwg2ZzBS/+qtLF2WBYzzgDNeSyFd7CbGItw==", null, false, "b5c2ac96-58dc-4281-94dc-8cbd998bf24b", false, "lore_vetto11", new DateTime(1998, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lorenzo", "Vettori" });
        }
    }
}
