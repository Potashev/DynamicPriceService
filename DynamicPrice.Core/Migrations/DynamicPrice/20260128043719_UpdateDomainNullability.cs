using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicPrice.Core.Migrations.DynamicPrice;

/// <inheritdoc />
public partial class UpdateDomainNullability : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropForeignKey(
			name: "FK_PriceRules_Companies_CompanyId",
			table: "PriceRules");

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

		migrationBuilder.AlterColumn<int>(
			name: "CompanyId",
			table: "PriceRules",
			type: "int",
			nullable: false,
			defaultValue: 0,
			oldClrType: typeof(int),
			oldType: "int",
			oldNullable: true);

		migrationBuilder.AlterColumn<DateTime>(
			name: "Date",
			table: "PriceDynamics",
			type: "datetime2",
			nullable: false,
			defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
			oldClrType: typeof(DateTime),
			oldType: "datetime2",
			oldNullable: true);

		migrationBuilder.AlterColumn<int>(
			name: "ReceiveKey",
			table: "Orders",
			type: "int",
			nullable: true,
			oldClrType: typeof(int),
			oldType: "int");

		migrationBuilder.AlterColumn<DateTime>(
			name: "OrderDate",
			table: "Orders",
			type: "datetime2",
			nullable: false,
			defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
			oldClrType: typeof(DateTime),
			oldType: "datetime2",
			oldNullable: true);

		migrationBuilder.AddForeignKey(
			name: "FK_PriceRules_Companies_CompanyId",
			table: "PriceRules",
			column: "CompanyId",
			principalTable: "Companies",
			principalColumn: "CompanyId",
			onDelete: ReferentialAction.Cascade);

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
			name: "FK_PriceRules_Companies_CompanyId",
			table: "PriceRules");

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

		migrationBuilder.AlterColumn<int>(
			name: "CompanyId",
			table: "PriceRules",
			type: "int",
			nullable: true,
			oldClrType: typeof(int),
			oldType: "int");

		migrationBuilder.AlterColumn<DateTime>(
			name: "Date",
			table: "PriceDynamics",
			type: "datetime2",
			nullable: true,
			oldClrType: typeof(DateTime),
			oldType: "datetime2");

		migrationBuilder.AlterColumn<int>(
			name: "ReceiveKey",
			table: "Orders",
			type: "int",
			nullable: false,
			defaultValue: 0,
			oldClrType: typeof(int),
			oldType: "int",
			oldNullable: true);

		migrationBuilder.AlterColumn<DateTime>(
			name: "OrderDate",
			table: "Orders",
			type: "datetime2",
			nullable: true,
			oldClrType: typeof(DateTime),
			oldType: "datetime2");

		migrationBuilder.AddForeignKey(
			name: "FK_PriceRules_Companies_CompanyId",
			table: "PriceRules",
			column: "CompanyId",
			principalTable: "Companies",
			principalColumn: "CompanyId");

		migrationBuilder.AddForeignKey(
			name: "FK_Products_Companies_CompanyId",
			table: "Products",
			column: "CompanyId",
			principalTable: "Companies",
			principalColumn: "CompanyId");
	}
}
