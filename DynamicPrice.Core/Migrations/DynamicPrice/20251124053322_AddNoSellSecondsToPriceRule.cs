using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicPrice.Core.Migrations.DynamicPrice;

/// <inheritdoc />
public partial class AddNoSellSecondsToPriceRule : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "NoSellTime",
			table: "PriceRules");

		migrationBuilder.AddColumn<int>(
			name: "NoSellSeconds",
			table: "PriceRules",
			type: "int",
			nullable: false,
			defaultValue: 0);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "NoSellSeconds",
			table: "PriceRules");

		migrationBuilder.AddColumn<TimeSpan>(
			name: "NoSellTime",
			table: "PriceRules",
			type: "time",
			nullable: true);
	}
}
