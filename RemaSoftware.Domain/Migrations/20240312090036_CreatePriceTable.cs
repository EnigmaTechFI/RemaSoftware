using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class CreatePriceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperationID1",
                table: "Operations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Price",
                columns: table => new
                {
                    PriceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PriceVal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Price", x => x.PriceID);
                    table.ForeignKey(
                        name: "FK_Price_Operations_OperationID",
                        column: x => x.OperationID,
                        principalTable: "Operations",
                        principalColumn: "OperationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Price_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Operations_OperationID1",
                table: "Operations",
                column: "OperationID1");

            migrationBuilder.CreateIndex(
                name: "IX_Price_OperationID",
                table: "Price",
                column: "OperationID");

            migrationBuilder.CreateIndex(
                name: "IX_Price_ProductID",
                table: "Price",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Operations_OperationID1",
                table: "Operations",
                column: "OperationID1",
                principalTable: "Operations",
                principalColumn: "OperationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Operations_OperationID1",
                table: "Operations");

            migrationBuilder.DropTable(
                name: "Price");

            migrationBuilder.DropIndex(
                name: "IX_Operations_OperationID1",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "OperationID1",
                table: "Operations");
        }
    }
}
