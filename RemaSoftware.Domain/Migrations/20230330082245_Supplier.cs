using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class Supplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pec",
                table: "Clients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    P_Iva = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StreetNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Cap = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Nation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nation_ISO = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Pec = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    SDI = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    FC_SupplierID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Pec",
                table: "Clients");
        }
    }
}
