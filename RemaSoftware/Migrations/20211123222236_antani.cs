using Microsoft.EntityFrameworkCore.Migrations;

namespace RemaSoftware.Migrations
{
    public partial class antani : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1,
                oldNullable: true);
        }
    }
}
