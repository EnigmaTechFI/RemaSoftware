using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class FixDDTSupplierAssociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DdtSupplierAssociations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ddt_Supplier_ID = table.Column<int>(type: "int", nullable: false),
                    Ddt_In_ID = table.Column<int>(type: "int", nullable: false),
                    NumberPieces = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DdtSupplierAssociations", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_DdtSupplierAssociations_Ddt_Supplier_ID",
                table: "DdtSupplierAssociations",
                column: "Ddt_Supplier_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DdtSupplierAssociations");
        }
    }
}
