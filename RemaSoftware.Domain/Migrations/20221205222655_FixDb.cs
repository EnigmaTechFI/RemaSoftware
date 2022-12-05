using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Migrations
{
    public partial class FixDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Operations_Orders_OrderID",
                table: "Order_Operations");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Clients_ClientID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClient_AspNetUsers_MyUserID",
                table: "UserClient");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClient_Clients_ClientID",
                table: "UserClient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClient",
                table: "UserClient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "UserClient",
                newName: "UserClients");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Order");

            migrationBuilder.RenameIndex(
                name: "IX_UserClient_MyUserID",
                table: "UserClients",
                newName: "IX_UserClients_MyUserID");

            migrationBuilder.RenameIndex(
                name: "IX_UserClient_ClientID",
                table: "UserClients",
                newName: "IX_UserClients_ClientID");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_ClientID",
                table: "Order",
                newName: "IX_Order_ClientID");

            migrationBuilder.AlterColumn<string>(
                name: "Image_URL",
                table: "Products",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(70)",
                oldMaxLength: 70);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClients",
                table: "UserClients",
                columns: new[] { "MyUserID", "ClientID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Clients_ClientID",
                table: "Order",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ClientID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Operations_Order_OrderID",
                table: "Order_Operations",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClients_AspNetUsers_MyUserID",
                table: "UserClients",
                column: "MyUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClients_Clients_ClientID",
                table: "UserClients",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ClientID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Clients_ClientID",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Operations_Order_OrderID",
                table: "Order_Operations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClients_AspNetUsers_MyUserID",
                table: "UserClients");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClients_Clients_ClientID",
                table: "UserClients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClients",
                table: "UserClients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.RenameTable(
                name: "UserClients",
                newName: "UserClient");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders");

            migrationBuilder.RenameIndex(
                name: "IX_UserClients_MyUserID",
                table: "UserClient",
                newName: "IX_UserClient_MyUserID");

            migrationBuilder.RenameIndex(
                name: "IX_UserClients_ClientID",
                table: "UserClient",
                newName: "IX_UserClient_ClientID");

            migrationBuilder.RenameIndex(
                name: "IX_Order_ClientID",
                table: "Orders",
                newName: "IX_Orders_ClientID");

            migrationBuilder.AlterColumn<string>(
                name: "Image_URL",
                table: "Products",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(70)",
                oldMaxLength: 70,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClient",
                table: "UserClient",
                columns: new[] { "MyUserID", "ClientID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Operations_Orders_OrderID",
                table: "Order_Operations",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Clients_ClientID",
                table: "Orders",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ClientID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClient_AspNetUsers_MyUserID",
                table: "UserClient",
                column: "MyUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClient_Clients_ClientID",
                table: "UserClient",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ClientID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
