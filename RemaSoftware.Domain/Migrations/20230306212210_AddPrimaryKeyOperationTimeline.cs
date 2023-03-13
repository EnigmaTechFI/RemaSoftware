using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Domain.Migrations
{
    public partial class AddPrimaryKeyOperationTimeline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperationTimelines",
                columns: table => new
                {
                    OperationTimelineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchOperationID = table.Column<int>(type: "int", nullable: false),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    SubBatchID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationTimelines", x => x.OperationTimelineID);
                    table.ForeignKey(
                        name: "FK_OperationTimelines_BatchOperations_BatchOperationID",
                        column: x => x.BatchOperationID,
                        principalTable: "BatchOperations",
                        principalColumn: "BatchOperationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperationTimelines_SubBatches_SubBatchID",
                        column: x => x.SubBatchID,
                        principalTable: "SubBatches",
                        principalColumn: "SubBatchID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationTimelines_BatchOperationID",
                table: "OperationTimelines",
                column: "BatchOperationID");

            migrationBuilder.CreateIndex(
                name: "IX_OperationTimelines_SubBatchID",
                table: "OperationTimelines",
                column: "SubBatchID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperationTimelines");
        }
    }
}
