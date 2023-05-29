using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class DdtTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ddt_TemplateID",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ddt_Templates",
                columns: table => new
                {
                    Ddt_Template_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FC_Template_ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ddt_Templates", x => x.Ddt_Template_ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Ddt_TemplateID",
                table: "Clients",
                column: "Ddt_TemplateID");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Ddt_Templates_Ddt_TemplateID",
                table: "Clients",
                column: "Ddt_TemplateID",
                principalTable: "Ddt_Templates",
                principalColumn: "Ddt_Template_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Ddt_Templates_Ddt_TemplateID",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "Ddt_Templates");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Ddt_TemplateID",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Ddt_TemplateID",
                table: "Clients");
        }
    }
}
