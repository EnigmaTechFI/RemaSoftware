using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class MultipleOperation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Operations_OperationID1",
                table: "Operations");

            migrationBuilder.DropForeignKey(
                name: "FK_Prices_Operations_OperationID",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_Prices_OperationID",
                table: "Prices");

            migrationBuilder.DropIndex(
                name: "IX_Operations_OperationID1",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "OperationID",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "OperationID1",
                table: "Operations");

            migrationBuilder.CreateTable(
                name: "PriceOperations",
                columns: table => new
                {
                    Price_ID = table.Column<int>(type: "int", nullable: false),
                    Operation_ID = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    PriceID = table.Column<int>(type: "int", nullable: true),
                    OperationID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceOperations", x => new { x.Price_ID, x.Operation_ID });
                    table.ForeignKey(
                        name: "FK_PriceOperations_Operations_OperationID",
                        column: x => x.OperationID,
                        principalTable: "Operations",
                        principalColumn: "OperationID");
                    table.ForeignKey(
                        name: "FK_PriceOperations_Prices_PriceID",
                        column: x => x.PriceID,
                        principalTable: "Prices",
                        principalColumn: "PriceID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceOperations_OperationID",
                table: "PriceOperations",
                column: "OperationID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceOperations_PriceID",
                table: "PriceOperations",
                column: "PriceID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceOperations");

            migrationBuilder.AddColumn<int>(
                name: "OperationID",
                table: "Prices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OperationID1",
                table: "Operations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prices_OperationID",
                table: "Prices",
                column: "OperationID");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_OperationID1",
                table: "Operations",
                column: "OperationID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Operations_OperationID1",
                table: "Operations",
                column: "OperationID1",
                principalTable: "Operations",
                principalColumn: "OperationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Prices_Operations_OperationID",
                table: "Prices",
                column: "OperationID",
                principalTable: "Operations",
                principalColumn: "OperationID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
