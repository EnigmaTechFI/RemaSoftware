using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class UpdateQualityControl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberFreeRepair",
                table: "Ddts_In",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberFreeRepair",
                table: "Ddt_Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberFreeRepair",
                table: "Ddts_In");

            migrationBuilder.DropColumn(
                name: "NumberFreeRepair",
                table: "Ddt_Suppliers");
        }
    }
}
