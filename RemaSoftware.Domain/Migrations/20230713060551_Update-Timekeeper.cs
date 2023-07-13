using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class UpdateTimekeeper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "number",
                table: "Employees",
                newName: "Number");

            migrationBuilder.AddColumn<bool>(
                name: "Extraordinary",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extraordinary",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Employees",
                newName: "number");
        }
    }
}
