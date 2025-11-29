using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicPriceCore.Migrations.DynamicPrice
{
	/// <inheritdoc />
	public partial class AddOrderNumber : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Number",
				table: "Orders",
				type: "nvarchar(20)",
				maxLength: 20,
				nullable: false,
				defaultValue: "");

			migrationBuilder.CreateIndex(
				name: "IX_Orders_Number",
				table: "Orders",
				column: "Number",
				unique: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_Orders_Number",
				table: "Orders");

			migrationBuilder.DropColumn(
				name: "Number",
				table: "Orders");
		}
	}
}
