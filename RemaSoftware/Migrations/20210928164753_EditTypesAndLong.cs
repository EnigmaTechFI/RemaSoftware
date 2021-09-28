using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemaSoftware.Migrations
{
    public partial class EditTypesAndLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "de58865f-b56d-4845-9f91-2c900a551df2");

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

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Price_Uni",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Price_Tot",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<string>(
                name: "StreetNumber",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "P_Iva",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cap",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "Birthday", "Name", "Surname" },
                values: new object[] { "516e55b0-f1f3-4678-b0d0-11f9258139a2", 0, "62d81fd9-44cd-4347-906c-3727b579e1c7", "MyUser", "lorenzo.vettori11@gmail.com", false, true, null, "LORENZO.VETTORI11@GMAIL.COM", "LORE_VETTO11", "AQAAAAEAACcQAAAAEG1b5y1qUW/c9zYT4/tUl5N/mIKSbsFRHyxkENKnQXu2CzmDXqc/j0Bammt1pQLSvA==", null, false, "ba7100f0-0cd5-44a8-a57b-f63c92d33c98", false, "lore_vetto11", new DateTime(1998, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lorenzo", "Vettori" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "516e55b0-f1f3-4678-b0d0-11f9258139a2");

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

            migrationBuilder.AlterColumn<int>(
                name: "SKU",
                table: "Orders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<float>(
                name: "Price_Uni",
                table: "Orders",
                type: "real",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<float>(
                name: "Price_Tot",
                table: "Orders",
                type: "real",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "StreetNumber",
                table: "Clients",
                type: "nvarchar(1)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Clients",
                type: "nvarchar(1)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "P_Iva",
                table: "Clients",
                type: "nvarchar(1)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Cap",
                table: "Clients",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "Birthday", "Name", "Surname" },
                values: new object[] { "de58865f-b56d-4845-9f91-2c900a551df2", 0, "fdbcc503-a687-4b21-a72c-b6f1c4f96e81", "MyUser", "lorenzo.vettori11@gmail.com", false, true, null, "LORENZO.VETTORI11@GMAIL.COM", "LORE_VETTO11", "AQAAAAEAACcQAAAAEB+rgvh3ehhvjUKKqt7iqrEPmP+KCpKIdSKd/i5QUAl52ns53w0wE2W+bwkBl5Lrzw==", null, false, "510af7a0-b16b-4d7f-b4da-0e36a76890c5", false, "lore_vetto11", new DateTime(1998, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lorenzo", "Vettori" });
        }
    }
}
