using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class PriceOperatoionTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Prices_PriceID",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_Operations_PriceID",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "PriceID",
                table: "Operations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriceID",
                table: "Operations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_PriceID",
                table: "Operations",
                column: "PriceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Prices_PriceID",
                table: "Operations",
                column: "PriceID",
                principalTable: "Prices",
                principalColumn: "PriceID");
        }
    }
}
