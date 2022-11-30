using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemaSoftware.Migrations
{
    public partial class BatchTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationTimelines_ProductOperations_ProductOperationDdt_In_ID_ProductOperationOperationID",
                table: "OperationTimelines");

            migrationBuilder.DropTable(
                name: "ProductOperations");

            migrationBuilder.DropIndex(
                name: "IX_OperationTimelines_ProductOperationDdt_In_ID_ProductOperationOperationID",
                table: "OperationTimelines");

            migrationBuilder.DropColumn(
                name: "Price_Uni",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductOperationDdt_In_ID",
                table: "OperationTimelines");

            migrationBuilder.DropColumn(
                name: "ProductOperationID",
                table: "OperationTimelines");

            migrationBuilder.RenameColumn(
                name: "ProductOperationOperationID",
                table: "OperationTimelines",
                newName: "BatchOperationID");

            migrationBuilder.AddColumn<int>(
                name: "BatchOperationsBatchID",
                table: "OperationTimelines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BatchOperationsOperationID",
                table: "OperationTimelines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BatchID",
                table: "Ddt_In",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Batch",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QrCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Price_Uni = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batch", x => x.BatchId);
                });

            migrationBuilder.CreateTable(
                name: "BatchOperations",
                columns: table => new
                {
                    BatchID = table.Column<int>(type: "int", nullable: false),
                    OperationID = table.Column<int>(type: "int", nullable: false),
                    Ordering = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    NumberMissingPiece = table.Column<int>(type: "int", nullable: false),
                    NumberWastePiece = table.Column<int>(type: "int", nullable: false),
                    NumberLostPiece = table.Column<int>(type: "int", nullable: false),
                    ExtraPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExtraPriceIsPending = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchOperations", x => new { x.BatchID, x.OperationID });
                    table.ForeignKey(
                        name: "FK_BatchOperations_Batch_BatchID",
                        column: x => x.BatchID,
                        principalTable: "Batch",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BatchOperations_Operations_OperationID",
                        column: x => x.OperationID,
                        principalTable: "Operations",
                        principalColumn: "OperationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationTimelines_BatchOperationsBatchID_BatchOperationsOperationID",
                table: "OperationTimelines",
                columns: new[] { "BatchOperationsBatchID", "BatchOperationsOperationID" });

            migrationBuilder.CreateIndex(
                name: "IX_Ddt_In_BatchID",
                table: "Ddt_In",
                column: "BatchID");

            migrationBuilder.CreateIndex(
                name: "IX_BatchOperations_OperationID",
                table: "BatchOperations",
                column: "OperationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ddt_In_Batch_BatchID",
                table: "Ddt_In",
                column: "BatchID",
                principalTable: "Batch",
                principalColumn: "BatchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OperationTimelines_BatchOperations_BatchOperationsBatchID_BatchOperationsOperationID",
                table: "OperationTimelines",
                columns: new[] { "BatchOperationsBatchID", "BatchOperationsOperationID" },
                principalTable: "BatchOperations",
                principalColumns: new[] { "BatchID", "OperationID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ddt_In_Batch_BatchID",
                table: "Ddt_In");

            migrationBuilder.DropForeignKey(
                name: "FK_OperationTimelines_BatchOperations_BatchOperationsBatchID_BatchOperationsOperationID",
                table: "OperationTimelines");

            migrationBuilder.DropTable(
                name: "BatchOperations");

            migrationBuilder.DropTable(
                name: "Batch");

            migrationBuilder.DropIndex(
                name: "IX_OperationTimelines_BatchOperationsBatchID_BatchOperationsOperationID",
                table: "OperationTimelines");

            migrationBuilder.DropIndex(
                name: "IX_Ddt_In_BatchID",
                table: "Ddt_In");

            migrationBuilder.DropColumn(
                name: "BatchOperationsBatchID",
                table: "OperationTimelines");

            migrationBuilder.DropColumn(
                name: "BatchOperationsOperationID",
                table: "OperationTimelines");

            migrationBuilder.DropColumn(
                name: "BatchID",
                table: "Ddt_In");

            migrationBuilder.RenameColumn(
                name: "BatchOperationID",
                table: "OperationTimelines",
                newName: "ProductOperationOperationID");

            migrationBuilder.AddColumn<decimal>(
                name: "Price_Uni",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProductOperationDdt_In_ID",
                table: "OperationTimelines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductOperationID",
                table: "OperationTimelines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProductOperations",
                columns: table => new
                {
                    Ddt_In_ID = table.Column<int>(type: "int", nullable: false),
                    OperationID = table.Column<int>(type: "int", nullable: false),
                    ExtraPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExtraPriceIsPending = table.Column<bool>(type: "bit", nullable: true),
                    NumberLostPiece = table.Column<int>(type: "int", nullable: false),
                    NumberMissingPiece = table.Column<int>(type: "int", nullable: false),
                    NumberWastePiece = table.Column<int>(type: "int", nullable: false),
                    Ordering = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOperations", x => new { x.Ddt_In_ID, x.OperationID });
                    table.ForeignKey(
                        name: "FK_ProductOperations_Ddt_In_Ddt_In_ID",
                        column: x => x.Ddt_In_ID,
                        principalTable: "Ddt_In",
                        principalColumn: "Ddt_In_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductOperations_Operations_OperationID",
                        column: x => x.OperationID,
                        principalTable: "Operations",
                        principalColumn: "OperationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperationTimelines_ProductOperationDdt_In_ID_ProductOperationOperationID",
                table: "OperationTimelines",
                columns: new[] { "ProductOperationDdt_In_ID", "ProductOperationOperationID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOperations_OperationID",
                table: "ProductOperations",
                column: "OperationID");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationTimelines_ProductOperations_ProductOperationDdt_In_ID_ProductOperationOperationID",
                table: "OperationTimelines",
                columns: new[] { "ProductOperationDdt_In_ID", "ProductOperationOperationID" },
                principalTable: "ProductOperations",
                principalColumns: new[] { "Ddt_In_ID", "OperationID" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
