using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class AddResoScartoNonLavorato : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResoScarto",
                table: "Label",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberReturnDiscard",
                table: "Ddts_In",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberResoScarto",
                table: "Ddt_Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResoScarto",
                table: "Label");

            migrationBuilder.DropColumn(
                name: "NumberReturnDiscard",
                table: "Ddts_In");

            migrationBuilder.DropColumn(
                name: "NumberResoScarto",
                table: "Ddt_Suppliers");
        }
    }
}
