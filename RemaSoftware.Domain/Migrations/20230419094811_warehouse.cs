using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class warehouse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Warehouse_Stocks");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Warehouse_Stocks");

            migrationBuilder.AddColumn<string>(
                name: "Measure_Unit",
                table: "Warehouse_Stocks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Product_Code",
                table: "Warehouse_Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Reorder_Limit",
                table: "Warehouse_Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierID",
                table: "Warehouse_Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Stock_Histories",
                columns: table => new
                {
                    Stock_HistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entry = table.Column<bool>(type: "bit", nullable: false),
                    Number_Piece = table.Column<int>(type: "int", nullable: false),
                    Warehouse_StockID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock_Histories", x => x.Stock_HistoryID);
                    table.ForeignKey(
                        name: "FK_Stock_Histories_Warehouse_Stocks_Warehouse_StockID",
                        column: x => x.Warehouse_StockID,
                        principalTable: "Warehouse_Stocks",
                        principalColumn: "Warehouse_StockID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Stocks_SupplierID",
                table: "Warehouse_Stocks",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_Histories_Warehouse_StockID",
                table: "Stock_Histories",
                column: "Warehouse_StockID");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_Stocks_Suppliers_SupplierID",
                table: "Warehouse_Stocks",
                column: "SupplierID",
                principalTable: "Suppliers",
                principalColumn: "SupplierID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_Stocks_Suppliers_SupplierID",
                table: "Warehouse_Stocks");

            migrationBuilder.DropTable(
                name: "Stock_Histories");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_Stocks_SupplierID",
                table: "Warehouse_Stocks");

            migrationBuilder.DropColumn(
                name: "Measure_Unit",
                table: "Warehouse_Stocks");

            migrationBuilder.DropColumn(
                name: "Product_Code",
                table: "Warehouse_Stocks");

            migrationBuilder.DropColumn(
                name: "Reorder_Limit",
                table: "Warehouse_Stocks");

            migrationBuilder.DropColumn(
                name: "SupplierID",
                table: "Warehouse_Stocks");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Warehouse_Stocks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Warehouse_Stocks",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
