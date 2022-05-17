using Microsoft.EntityFrameworkCore.Migrations;

namespace RemaSoftware.Domain.Migrations
{
    public partial class AddOrderingOperation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ordering",
                table: "Order_Operations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ordering",
                table: "Order_Operations");
        }
    }
}
