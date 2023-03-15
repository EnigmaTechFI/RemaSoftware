using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class DDTPiecesDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberLostPiece",
                table: "BatchOperations");

            migrationBuilder.DropColumn(
                name: "NumberMissingPiece",
                table: "BatchOperations");

            migrationBuilder.DropColumn(
                name: "NumberWastePiece",
                table: "BatchOperations");

            migrationBuilder.AddColumn<int>(
                name: "NumberLostPiece",
                table: "Ddts_In",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberMissingPiece",
                table: "Ddts_In",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberWastePiece",
                table: "Ddts_In",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberLostPiece",
                table: "Ddts_In");

            migrationBuilder.DropColumn(
                name: "NumberMissingPiece",
                table: "Ddts_In");

            migrationBuilder.DropColumn(
                name: "NumberWastePiece",
                table: "Ddts_In");

            migrationBuilder.AddColumn<int>(
                name: "NumberLostPiece",
                table: "BatchOperations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberMissingPiece",
                table: "BatchOperations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberWastePiece",
                table: "BatchOperations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
