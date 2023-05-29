using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class DDTSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "OperationTimelines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Ddt_Suppliers",
                columns: table => new
                {
                    Ddt_Supplier_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataReIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataOut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cost_Uni = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FC_Ddt_Supplier_ID = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Number_Piece = table.Column<int>(type: "int", nullable: false),
                    NumberReInPiece = table.Column<int>(type: "int", nullable: false),
                    NumberWastePiece = table.Column<int>(type: "int", nullable: false),
                    NumberLostPiece = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    IsReso = table.Column<bool>(type: "bit", nullable: false),
                    OperationTimelineID = table.Column<int>(type: "int", nullable: false),
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ddt_Suppliers", x => x.Ddt_Supplier_ID);
                    table.ForeignKey(
                        name: "FK_Ddt_Suppliers_OperationTimelines_OperationTimelineID",
                        column: x => x.OperationTimelineID,
                        principalTable: "OperationTimelines",
                        principalColumn: "OperationTimelineID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ddt_Suppliers_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ddt_Suppliers_OperationTimelineID",
                table: "Ddt_Suppliers",
                column: "OperationTimelineID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ddt_Suppliers_SupplierID",
                table: "Ddt_Suppliers",
                column: "SupplierID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ddt_Suppliers");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "OperationTimelines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
