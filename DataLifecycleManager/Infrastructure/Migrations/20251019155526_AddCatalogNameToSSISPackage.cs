using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLifecycleManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogNameToSSISPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CatalogName",
                table: "SSISPackages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatalogName",
                table: "SSISPackages");
        }
    }
}
