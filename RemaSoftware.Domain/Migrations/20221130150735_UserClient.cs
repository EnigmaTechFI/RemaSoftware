using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Migrations
{
    public partial class UserClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Clients_ClientID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ClientID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ClientID",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "UserClient",
                columns: table => new
                {
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    MyUserID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClient", x => new { x.MyUserID, x.ClientID });
                    table.ForeignKey(
                        name: "FK_UserClient_AspNetUsers_MyUserID",
                        column: x => x.MyUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserClient_Clients_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Clients",
                        principalColumn: "ClientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserClient_ClientID",
                table: "UserClient",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_UserClient_MyUserID",
                table: "UserClient",
                column: "MyUserID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClient");

            migrationBuilder.AddColumn<int>(
                name: "ClientID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ClientID",
                table: "AspNetUsers",
                column: "ClientID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Clients_ClientID",
                table: "AspNetUsers",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ClientID");
        }
    }
}
