using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicPrice.Core.Migrations.DynamicPrice
{
    /// <inheritdoc />
    public partial class AddActiveCompanyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveCompanies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMonitoring = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveCompanies", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_ActiveCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveCompanies");
        }
    }
}
