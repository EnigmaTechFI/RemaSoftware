using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class DDTSupplierAssociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number_Piece_ToSupplier",
                table: "Ddts_In",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DdtSupplierAssociations",
                columns: table => new
                {
                    Ddt_Supplier_ID = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Ddt_In_ID = table.Column<int>(type: "int", nullable: false),
                    NumberPieces = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DdtSupplierAssociations", x => x.Ddt_Supplier_ID);
                    table.ForeignKey(
                        name: "FK_DdtSupplierAssociations_Ddt_Suppliers_Ddt_Supplier_ID",
                        column: x => x.Ddt_Supplier_ID,
                        principalTable: "Ddt_Suppliers",
                        principalColumn: "Ddt_Supplier_ID");
                    table.ForeignKey(
                        name: "FK_DdtSupplierAssociations_Ddts_In_Ddt_In_ID",
                        column: x => x.Ddt_In_ID,
                        principalTable: "Ddts_In",
                        principalColumn: "Ddt_In_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DdtSupplierAssociations_Ddt_In_ID",
                table: "DdtSupplierAssociations",
                column: "Ddt_In_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DdtSupplierAssociations");

            migrationBuilder.DropColumn(
                name: "Number_Piece_ToSupplier",
                table: "Ddts_In");
        }
    }
}
