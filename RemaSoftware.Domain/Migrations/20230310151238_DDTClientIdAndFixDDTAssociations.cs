using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class DDTClientIdAndFixDDTAssociations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Ddt_Associations",
                table: "Ddt_Associations");

            migrationBuilder.AddColumn<int>(
                name: "ClientID",
                table: "Ddts_Out",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Ddt_Association_ID",
                table: "Ddt_Associations",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Ddt_Associations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ddt_Associations",
                table: "Ddt_Associations",
                column: "Ddt_Association_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Ddts_Out_ClientID",
                table: "Ddts_Out",
                column: "ClientID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ddts_Out_Clients_ClientID",
                table: "Ddts_Out",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ClientID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ddts_Out_Clients_ClientID",
                table: "Ddts_Out");

            migrationBuilder.DropIndex(
                name: "IX_Ddts_Out_ClientID",
                table: "Ddts_Out");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ddt_Associations",
                table: "Ddt_Associations");

            migrationBuilder.DropColumn(
                name: "ClientID",
                table: "Ddts_Out");

            migrationBuilder.DropColumn(
                name: "Ddt_Association_ID",
                table: "Ddt_Associations");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Ddt_Associations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ddt_Associations",
                table: "Ddt_Associations",
                columns: new[] { "Ddt_In_ID", "Ddt_Out_ID" });
        }
    }
}
