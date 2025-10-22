using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLifecycleManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSSISPackageExecutionIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SSISPackageExecutions_SSISPackages_SSISPackageId",
                table: "SSISPackageExecutions");

            migrationBuilder.AddColumn<int>(
                name: "SSISPackageId1",
                table: "SSISPackageExecutions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SSISPackageExecutions_CatalogExecutionId",
                table: "SSISPackageExecutions",
                column: "CatalogExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_SSISPackageExecutions_CreatedAt",
                table: "SSISPackageExecutions",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_SSISPackageExecutions_ExecutedBy",
                table: "SSISPackageExecutions",
                column: "ExecutedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SSISPackageExecutions_SSISPackageId1",
                table: "SSISPackageExecutions",
                column: "SSISPackageId1");

            migrationBuilder.CreateIndex(
                name: "IX_SSISPackageExecutions_Status_CreatedAt",
                table: "SSISPackageExecutions",
                columns: new[] { "Status", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.AddForeignKey(
                name: "FK_SSISPackageExecutions_SSISPackages_SSISPackageId",
                table: "SSISPackageExecutions",
                column: "SSISPackageId",
                principalTable: "SSISPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SSISPackageExecutions_SSISPackages_SSISPackageId1",
                table: "SSISPackageExecutions",
                column: "SSISPackageId1",
                principalTable: "SSISPackages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SSISPackageExecutions_SSISPackages_SSISPackageId",
                table: "SSISPackageExecutions");

            migrationBuilder.DropForeignKey(
                name: "FK_SSISPackageExecutions_SSISPackages_SSISPackageId1",
                table: "SSISPackageExecutions");

            migrationBuilder.DropIndex(
                name: "IX_SSISPackageExecutions_CatalogExecutionId",
                table: "SSISPackageExecutions");

            migrationBuilder.DropIndex(
                name: "IX_SSISPackageExecutions_CreatedAt",
                table: "SSISPackageExecutions");

            migrationBuilder.DropIndex(
                name: "IX_SSISPackageExecutions_ExecutedBy",
                table: "SSISPackageExecutions");

            migrationBuilder.DropIndex(
                name: "IX_SSISPackageExecutions_SSISPackageId1",
                table: "SSISPackageExecutions");

            migrationBuilder.DropIndex(
                name: "IX_SSISPackageExecutions_Status_CreatedAt",
                table: "SSISPackageExecutions");

            migrationBuilder.DropColumn(
                name: "SSISPackageId1",
                table: "SSISPackageExecutions");

            migrationBuilder.AddForeignKey(
                name: "FK_SSISPackageExecutions_SSISPackages_SSISPackageId",
                table: "SSISPackageExecutions",
                column: "SSISPackageId",
                principalTable: "SSISPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
