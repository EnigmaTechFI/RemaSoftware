using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemaSoftware.Migrations
{
    public partial class DatabaseRedifinition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clients",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FC_ClientID",
                table: "Clients",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "Clients",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nation_ISO",
                table: "Clients",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Clients",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientID",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ddt_Out",
                columns: table => new
                {
                    Ddt_Out_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(maxLength: 1, nullable: true),
                    FC_Ddt_Out_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ddt_Out", x => x.Ddt_Out_ID);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    RemaCode = table.Column<string>(maxLength: 30, nullable: true),
                    SKU = table.Column<string>(maxLength: 50, nullable: false),
                    Price_Uni = table.Column<decimal>(nullable: false),
                    Image_URL = table.Column<string>(maxLength: 70, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Products_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ddt_In",
                columns: table => new
                {
                    Ddt_In_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ddt_Out_ID = table.Column<int>(nullable: true),
                    ProductID = table.Column<int>(nullable: false),
                    DataIn = table.Column<DateTime>(nullable: false),
                    DataOut = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    FC_Ddt_In_ID = table.Column<string>(maxLength: 30, nullable: true),
                    Number_Piece = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Status = table.Column<string>(maxLength: 1, nullable: true),
                    IsReso = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ddt_In", x => x.Ddt_In_ID);
                    table.ForeignKey(
                        name: "FK_Ddt_In_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductOperations",
                columns: table => new
                {
                    Ddt_In_ID = table.Column<int>(nullable: false),
                    OperationID = table.Column<int>(nullable: false),
                    Ordering = table.Column<int>(nullable: false),
                    Status = table.Column<string>(maxLength: 1, nullable: true),
                    NumberMissingPiece = table.Column<int>(nullable: false),
                    NumberWastePiece = table.Column<int>(nullable: false),
                    NumberLostPiece = table.Column<int>(nullable: false),
                    ExtraPrice = table.Column<decimal>(nullable: true),
                    ExtraPriceIsPending = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOperations", x => new { x.Ddt_In_ID, x.OperationID });
                    table.ForeignKey(
                        name: "FK_ProductOperations_Ddt_In_Ddt_In_ID",
                        column: x => x.Ddt_In_ID,
                        principalTable: "Ddt_In",
                        principalColumn: "Ddt_In_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductOperations_Operations_OperationID",
                        column: x => x.OperationID,
                        principalTable: "Operations",
                        principalColumn: "OperationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationTimelines",
                columns: table => new
                {
                    OperationTimelineID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductOperationID = table.Column<int>(nullable: false),
                    ProductOperationDdt_In_ID = table.Column<int>(nullable: false),
                    ProductOperationOperationID = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationTimelines", x => x.OperationTimelineID);
                    table.ForeignKey(
                        name: "FK_OperationTimelines_ProductOperations_ProductOperationDdt_In_ID_ProductOperationOperationID",
                        columns: x => new { x.ProductOperationDdt_In_ID, x.ProductOperationOperationID },
                        principalTable: "ProductOperations",
                        principalColumns: new[] { "Ddt_In_ID", "OperationID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ClientID",
                table: "AspNetUsers",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Ddt_In_ProductID",
                table: "Ddt_In",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTimelines_ProductOperationDdt_In_ID_ProductOperationOperationID",
                table: "OperationTimelines",
                columns: new[] { "ProductOperationDdt_In_ID", "ProductOperationOperationID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOperations_OperationID",
                table: "ProductOperations",
                column: "OperationID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ClientID",
                table: "Products",
                column: "ClientID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Clients_ClientID",
                table: "AspNetUsers",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ClientID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Clients_ClientID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Ddt_Out");

            migrationBuilder.DropTable(
                name: "OperationTimelines");

            migrationBuilder.DropTable(
                name: "ProductOperations");

            migrationBuilder.DropTable(
                name: "Ddt_In");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ClientID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "FC_ClientID",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Nation_ISO",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ClientID",
                table: "AspNetUsers");
        }
    }
}
