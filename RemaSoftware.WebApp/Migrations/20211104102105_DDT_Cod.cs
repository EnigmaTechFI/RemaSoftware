using Microsoft.EntityFrameworkCore.Migrations;

namespace RemaSoftware.WebApp.Migrations
{
    public partial class DDT_Cod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DDT",
                table: "Orders",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DDT",
                table: "Orders");
        }
    }
}
