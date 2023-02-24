using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class AddRelDdtSubBatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ddts_In_Batches_BatchID",
                table: "Ddts_In");

            migrationBuilder.RenameColumn(
                name: "BatchID",
                table: "Ddts_In",
                newName: "SubBatchID");

            migrationBuilder.RenameIndex(
                name: "IX_Ddts_In_BatchID",
                table: "Ddts_In",
                newName: "IX_Ddts_In_SubBatchID");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SubBatches",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ddts_In_SubBatches_SubBatchID",
                table: "Ddts_In",
                column: "SubBatchID",
                principalTable: "SubBatches",
                principalColumn: "SubBatchID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ddts_In_SubBatches_SubBatchID",
                table: "Ddts_In");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SubBatches");

            migrationBuilder.RenameColumn(
                name: "SubBatchID",
                table: "Ddts_In",
                newName: "BatchID");

            migrationBuilder.RenameIndex(
                name: "IX_Ddts_In_SubBatchID",
                table: "Ddts_In",
                newName: "IX_Ddts_In_BatchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ddts_In_Batches_BatchID",
                table: "Ddts_In",
                column: "BatchID",
                principalTable: "Batches",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
