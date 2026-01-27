using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicPrice.Core.Migrations.DynamicPrice;

/// <inheritdoc />
public partial class AddedRequired : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropForeignKey(
			name: "FK_Products_Companies_CompanyId",
			table: "Products");

		migrationBuilder.AlterColumn<int>(
			name: "CompanyId",
			table: "Products",
			type: "int",
			nullable: false,
			defaultValue: 0,
			oldClrType: typeof(int),
			oldType: "int",
			oldNullable: true);

		migrationBuilder.AddForeignKey(
			name: "FK_Products_Companies_CompanyId",
			table: "Products",
			column: "CompanyId",
			principalTable: "Companies",
			principalColumn: "CompanyId",
			onDelete: ReferentialAction.Cascade);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropForeignKey(
			name: "FK_Products_Companies_CompanyId",
			table: "Products");

		migrationBuilder.AlterColumn<int>(
			name: "CompanyId",
			table: "Products",
			type: "int",
			nullable: true,
			oldClrType: typeof(int),
			oldType: "int");

		migrationBuilder.AddForeignKey(
			name: "FK_Products_Companies_CompanyId",
			table: "Products",
			column: "CompanyId",
			principalTable: "Companies",
			principalColumn: "CompanyId");
	}
}
