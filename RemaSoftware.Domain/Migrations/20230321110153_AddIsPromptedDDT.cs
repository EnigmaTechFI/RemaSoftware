using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class AddIsPromptedDDT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrompted",
                table: "Ddts_In",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrompted",
                table: "Ddts_In");
        }
    }
}
