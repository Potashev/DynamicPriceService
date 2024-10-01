using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicPriceCore.Migrations
{
	/// <inheritdoc />
	public partial class SeedTestData : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.InsertData(
				"Companies",
				["CompanyId", "Title"],
				new object[,]
				{
					{ 1, "Автозапчасти" },
					{ 2, "Копыта" }
				});

			migrationBuilder.InsertData(
				"Products",
				["ProductId", "CompanyId", "Title", "Price", "MinimumPrice", "Quantity", "Description", "LastSellTime"],
				new object[,]
				{
					{ 1, 1, "Моторное масло", 5000, 1000, 100, "Mobil 5w-30", DateTime.UtcNow },
					{ 2, 1, "Свечи", 1500, 1200, null, "NGK", DateTime.UtcNow },
					{ 3, 1, "Воздушный фильтр", 700, 300, 100, null, DateTime.UtcNow },
					{ 4, 1, "Ремень ГРМ", 3000, 1000, 10, null, DateTime.UtcNow },
					{ 5, 1, "Антифриз", 500, 200, 30, null, DateTime.UtcNow },
					{ 6, 2, "Хлеб", 50, 40, null, null, DateTime.UtcNow },
					{ 7, 2, "Молоко", 100, 80, null, null, DateTime.UtcNow },
				});

			migrationBuilder.InsertData(
				"PriceRules",
				["PriceRuleId", "CompanyId", "Increase", "Reduction", "NoSellTime"],
				new object[,]
				{
					{ 1, 1, 1, 1, new TimeSpan(0,0,10) },
					{ 2, 2, 1, 1, new TimeSpan(0,0,10) }
				});

			migrationBuilder.InsertData(
				"CompanyUsers",
				["CompanyId", "UserId"],
				new object[,]
				{
					{ 1, "1" },
					{ 2, "2" }
				});

			migrationBuilder.InsertData(
				"Customers",
				["CustomerId", "Name"],
				new object[,]
				{
					{ 1, "Customer1" }
				});

			//todo: add orders and price dynamics
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{

		}
	}
}
