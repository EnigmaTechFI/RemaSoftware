using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class ChangeFreeRepair : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberFreeRepair",
                table: "Ddts_In");

            migrationBuilder.AddColumn<bool>(
                name: "FreeRepair",
                table: "Ddts_In",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeRepair",
                table: "Ddts_In");

            migrationBuilder.AddColumn<int>(
                name: "NumberFreeRepair",
                table: "Ddts_In",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
