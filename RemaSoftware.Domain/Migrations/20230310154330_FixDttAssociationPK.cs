using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class FixDttAssociationPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ddt_Associations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Ddt_In_ID = table.Column<int>(type: "int", nullable: false),
                    Ddt_Out_ID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberPieces = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ddt_Associations", x => new { x.Ddt_In_ID, x.Ddt_Out_ID, x.ID });
                    table.ForeignKey(
                        name: "FK_Ddt_Associations_Ddts_In_Ddt_In_ID",
                        column: x => x.Ddt_In_ID,
                        principalTable: "Ddts_In",
                        principalColumn: "Ddt_In_ID");
                    table.ForeignKey(
                        name: "FK_Ddt_Associations_Ddts_Out_Ddt_Out_ID",
                        column: x => x.Ddt_Out_ID,
                        principalTable: "Ddts_Out",
                        principalColumn: "Ddt_Out_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ddt_Associations_Ddt_Out_ID",
                table: "Ddt_Associations",
                column: "Ddt_Out_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ddt_Associations");
        }
    }
}
