using Microsoft.EntityFrameworkCore.Migrations;

namespace RemaSoftware.Domain.Migrations
{
    public partial class ID_FattureInCloud : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ID_FattureInCloud",
                table: "Orders",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ID_FattureInCloud",
                table: "Orders");
        }
    }
}
