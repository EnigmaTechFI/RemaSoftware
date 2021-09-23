using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemaSoftware.Migrations
{
    public partial class Default : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "de6944ea-bf51-47b5-acb1-58e1a1978791");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    P_Iva = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    StreetNumber = table.Column<string>(nullable: true),
                    Cap = table.Column<int>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientID);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    OperationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.OperationID);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse_Stocks",
                columns: table => new
                {
                    Warehouse_StockID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Brand = table.Column<string>(nullable: true),
                    Number_Piece = table.Column<int>(nullable: false),
                    Price_Tot = table.Column<float>(nullable: false),
                    Price_Uni = table.Column<float>(nullable: false),
                    Size = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse_Stocks", x => x.Warehouse_StockID);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    ClientID = table.Column<int>(nullable: false),
                    Number_Piece = table.Column<int>(nullable: false),
                    DataIn = table.Column<DateTime>(nullable: false),
                    DataOut = table.Column<DateTime>(nullable: false),
                    SKU = table.Column<int>(nullable: false),
                    Image_URL = table.Column<string>(nullable: false),
                    Pdf_URL = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Flag_Fattureincloud = table.Column<bool>(nullable: false),
                    Price_Tot = table.Column<float>(nullable: false),
                    Price_Uni = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Orders_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order_Operations",
                columns: table => new
                {
                    OrderID = table.Column<int>(nullable: false),
                    OperationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Operations", x => new { x.OrderID, x.OperationID });
                    table.ForeignKey(
                        name: "FK_Order_Operations_Operations_OperationID",
                        column: x => x.OperationID,
                        principalTable: "Operations",
                        principalColumn: "OperationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_Operations_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "Birthday", "Name", "Surname" },
                values: new object[] { "de58865f-b56d-4845-9f91-2c900a551df2", 0, "fdbcc503-a687-4b21-a72c-b6f1c4f96e81", "MyUser", "lorenzo.vettori11@gmail.com", false, true, null, "LORENZO.VETTORI11@GMAIL.COM", "LORE_VETTO11", "AQAAAAEAACcQAAAAEB+rgvh3ehhvjUKKqt7iqrEPmP+KCpKIdSKd/i5QUAl52ns53w0wE2W+bwkBl5Lrzw==", null, false, "510af7a0-b16b-4d7f-b4da-0e36a76890c5", false, "lore_vetto11", new DateTime(1998, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lorenzo", "Vettori" });

            migrationBuilder.CreateIndex(
                name: "IX_Order_Operations_OperationID",
                table: "Order_Operations",
                column: "OperationID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientID",
                table: "Orders",
                column: "ClientID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order_Operations");

            migrationBuilder.DropTable(
                name: "Warehouse_Stocks");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "de58865f-b56d-4845-9f91-2c900a551df2");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "Birthday", "Name", "Surname" },
                values: new object[] { "de6944ea-bf51-47b5-acb1-58e1a1978791", 0, "ee429871-2884-41da-a5a5-d635141cb4b5", "MyUser", "lorenzo.vettori11@gmail.com", false, true, null, "LORENZO.VETTORI11@GMAIL.COM", "LORE_VETTO11", "AQAAAAEAACcQAAAAENNd1b34zR+qRvB4Xk2Pu3lrEIysCFx3ii0rVFWfz+I60UiPWZ9X2ejb+5FkWfJJ6w==", null, false, "9f883eba-e139-4da8-9529-fb903eaa660a", false, "lore_vetto11", new DateTime(1998, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lorenzo", "Vettori" });
        }
    }
}
