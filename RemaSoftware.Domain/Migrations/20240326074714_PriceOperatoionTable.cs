using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class PriceOperatoionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceOperations_Operations_OperationID",
                table: "PriceOperations");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceOperations_Prices_PriceID",
                table: "PriceOperations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceOperations",
                table: "PriceOperations");

            migrationBuilder.DropIndex(
                name: "IX_PriceOperations_PriceID",
                table: "PriceOperations");

            migrationBuilder.DropColumn(
                name: "Price_ID",
                table: "PriceOperations");

            migrationBuilder.DropColumn(
                name: "Operation_ID",
                table: "PriceOperations");

            migrationBuilder.AlterColumn<int>(
                name: "PriceID",
                table: "PriceOperations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OperationID",
                table: "PriceOperations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceOperations",
                table: "PriceOperations",
                columns: new[] { "PriceID", "OperationID" });

            migrationBuilder.AddForeignKey(
                name: "FK_PriceOperations_Operations_OperationID",
                table: "PriceOperations",
                column: "OperationID",
                principalTable: "Operations",
                principalColumn: "OperationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceOperations_Prices_PriceID",
                table: "PriceOperations",
                column: "PriceID",
                principalTable: "Prices",
                principalColumn: "PriceID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceOperations_Operations_OperationID",
                table: "PriceOperations");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceOperations_Prices_PriceID",
                table: "PriceOperations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceOperations",
                table: "PriceOperations");

            migrationBuilder.AlterColumn<int>(
                name: "OperationID",
                table: "PriceOperations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PriceID",
                table: "PriceOperations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Price_ID",
                table: "PriceOperations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Operation_ID",
                table: "PriceOperations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceOperations",
                table: "PriceOperations",
                columns: new[] { "Price_ID", "Operation_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_PriceOperations_PriceID",
                table: "PriceOperations",
                column: "PriceID");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceOperations_Operations_OperationID",
                table: "PriceOperations",
                column: "OperationID",
                principalTable: "Operations",
                principalColumn: "OperationID");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceOperations_Prices_PriceID",
                table: "PriceOperations",
                column: "PriceID",
                principalTable: "Prices",
                principalColumn: "PriceID");
        }
    }
}
