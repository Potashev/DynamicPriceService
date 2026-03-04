using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicPrice.Core.Migrations.DynamicPrice
{
	/// <inheritdoc />
	public partial class AddPriceHistoryLimit : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "PriceHistoryLimit",
				table: "Companies",
				type: "int",
				nullable: false,
				defaultValue: 0);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "PriceHistoryLimit",
				table: "Companies");
		}
	}
}
