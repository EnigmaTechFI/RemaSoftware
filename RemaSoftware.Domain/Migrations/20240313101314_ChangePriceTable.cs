using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class ChangePriceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Price_Operations_OperationID",
                table: "Price");

            migrationBuilder.DropForeignKey(
                name: "FK_Price_Products_ProductID",
                table: "Price");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Price",
                table: "Price");

            migrationBuilder.RenameTable(
                name: "Price",
                newName: "Prices");

            migrationBuilder.RenameIndex(
                name: "IX_Price_ProductID",
                table: "Prices",
                newName: "IX_Prices_ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_Price_OperationID",
                table: "Prices",
                newName: "IX_Prices_OperationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prices",
                table: "Prices",
                column: "PriceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_Operations_OperationID",
                table: "Prices",
                column: "OperationID",
                principalTable: "Operations",
                principalColumn: "OperationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_Products_ProductID",
                table: "Prices",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prices_Operations_OperationID",
                table: "Prices");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_Products_ProductID",
                table: "Prices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prices",
                table: "Prices");

            migrationBuilder.RenameTable(
                name: "Prices",
                newName: "Price");

            migrationBuilder.RenameIndex(
                name: "IX_Prices_ProductID",
                table: "Price",
                newName: "IX_Price_ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_Prices_OperationID",
                table: "Price",
                newName: "IX_Price_OperationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Price",
                table: "Price",
                column: "PriceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Price_Operations_OperationID",
                table: "Price",
                column: "OperationID",
                principalTable: "Operations",
                principalColumn: "OperationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Price_Products_ProductID",
                table: "Price",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
