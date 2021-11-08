using Microsoft.EntityFrameworkCore.Migrations;

namespace RemaSoftware.Migrations
{
    public partial class AddColumnNoteInOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Orders");
        }
    }
}
