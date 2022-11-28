using Microsoft.EntityFrameworkCore.Migrations;

namespace RemaSoftware.Migrations
{
    public partial class ProductFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "NumberPieces",
                table: "Ddt_Out",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Ddt_In",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberPieces",
                table: "Ddt_Out");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Ddt_In");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
