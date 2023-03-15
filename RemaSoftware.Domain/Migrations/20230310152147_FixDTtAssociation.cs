using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class FixDTtAssociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ddt_Associations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ddt_Associations",
                columns: table => new
                {
                    Ddt_Association_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ddt_In_ID1 = table.Column<int>(type: "int", nullable: true),
                    Ddt_Out_ID1 = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ddt_In_ID = table.Column<int>(type: "int", nullable: false),
                    Ddt_Out_ID = table.Column<int>(type: "int", nullable: false),
                    NumberPieces = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ddt_Associations", x => x.Ddt_Association_ID);
                    table.ForeignKey(
                        name: "FK_Ddt_Associations_Ddts_In_Ddt_In_ID1",
                        column: x => x.Ddt_In_ID1,
                        principalTable: "Ddts_In",
                        principalColumn: "Ddt_In_ID");
                    table.ForeignKey(
                        name: "FK_Ddt_Associations_Ddts_Out_Ddt_Out_ID1",
                        column: x => x.Ddt_Out_ID1,
                        principalTable: "Ddts_Out",
                        principalColumn: "Ddt_Out_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ddt_Associations_Ddt_In_ID1",
                table: "Ddt_Associations",
                column: "Ddt_In_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Ddt_Associations_Ddt_Out_ID1",
                table: "Ddt_Associations",
                column: "Ddt_Out_ID1");
        }
    }
}
