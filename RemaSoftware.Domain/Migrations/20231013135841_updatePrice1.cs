using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class updatePrice1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price_Uni",
                table: "Ddts_In",
                type: "decimal(16,3)",
                precision: 16,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PendingPrice",
                table: "Ddts_In",
                type: "decimal(16,3)",
                precision: 16,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price_Uni",
                table: "Ddts_In",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(16,3)",
                oldPrecision: 16,
                oldScale: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "PendingPrice",
                table: "Ddts_In",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(16,3)",
                oldPrecision: 16,
                oldScale: 3);
        }
    }
}
