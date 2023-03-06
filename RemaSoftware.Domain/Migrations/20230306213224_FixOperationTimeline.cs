using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class FixOperationTimeline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationTimelines_BatchOperations_BatchOperationID",
                table: "OperationTimelines");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationTimelines_BatchOperations_BatchOperationID",
                table: "OperationTimelines",
                column: "BatchOperationID",
                principalTable: "BatchOperations",
                principalColumn: "BatchOperationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationTimelines_BatchOperations_BatchOperationID",
                table: "OperationTimelines");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationTimelines_BatchOperations_BatchOperationID",
                table: "OperationTimelines",
                column: "BatchOperationID",
                principalTable: "BatchOperations",
                principalColumn: "BatchOperationID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
