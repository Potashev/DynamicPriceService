using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicPriceCore.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Добавляем роли
			migrationBuilder.InsertData(
				"AspNetRoles",
				new[] { "Id", "Name", "NormalizedName" },
				new object[,]
				{
			{ "1", "Customer", "CUSTOMER" },
			{ "2", "Manager", "MANAGER" }
				});

			// Добавляем компании
			migrationBuilder.InsertData(
				"Companies",
				new[] { "CompanyId", "Title" },
				new object[,]
				{
			{ 1, "Автозапчасти" },
			{ 2, "Копыта" }
				});

			// Добавляем товары
			migrationBuilder.InsertData(
				"Products",
				new[] { "ProductId", "CompanyId", "Title", "Price", "MinimumPrice", "Quantity", "Description", "LastSellTime" },
				new object[,]
				{
			{ 1, 1, "Моторное масло", 5000, 1000, 100, "Mobil 5w-30", DateTime.UtcNow },
			{ 2, 1, "Свечи", 1500, 1200, null, "NGK", DateTime.UtcNow },
			{ 3, 1, "Воздушный фильтр", 700, 300, 100, null, DateTime.UtcNow },
			{ 4, 1, "Ремень ГРМ", 3000, 1000, 10, null, DateTime.UtcNow },
			{ 5, 1, "Антифриз", 500, 200, 30, null, DateTime.UtcNow },
			{ 6, 2, "Хлеб", 50, 40, null, null, DateTime.UtcNow },
			{ 7, 2, "Молоко", 100, 80, null, null, DateTime.UtcNow }
				});

			// Добавляем правила ценообразования
			migrationBuilder.InsertData(
				"PriceRules",
				new[] { "PriceRuleId", "CompanyId", "Increase", "Reduction", "NoSellTime" },
				new object[,]
				{
			{ 1, 1, 1, 1, new TimeSpan(0,0,10) },
			{ 2, 2, 1, 1, new TimeSpan(0,0,10) }
				});

			// Привязываем пользователей к компаниям
			//migrationBuilder.InsertData(
			//	"CompanyUsers",
			//	new[] { "CompanyId", "UserId" },
			//	new object[,]
			//	{
			//{ 1, "1" },
			//{ 2, "2" }
			//	});

			// Добавляем тестового кастомера (без CustomerId)
			migrationBuilder.InsertData(
				"AspNetUsers",
				new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "AccessFailedCount", "Discriminator", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled" },
				new object[,]
				{
		{ "1001", "customer1", "CUSTOMER1", "customer@test.com", "CUSTOMER@TEST.COM", true, "HASHED_PASSWORD_HERE", "", 0, "Customer", false, false, true },
        { "1002", "manager1", "MANAGER1", "manager@test.com", "MANAGER@TEST.COM", true, "HASHED_PASSWORD_HERE", "", 0, "Manager", false, false, true },
		{ "450fdd56-43cd-4bd5-979f-27959450181c", "Nik1", "NIK1", "potashev.nik@gmail.com", "POTASHEV.NIK@GMAIL.COM", false, "AQAAAAIAAYagAAAAEG6s4gKJ02XTB/rYvujCUatoTzAxGfAneaZCBu2HYWhqRrksA4Zq/1vJqRXj382szQ==", "OM3OR6CWNA55FUGLG5VDANGFU2PYWOYU", 0, "Customer", false, false, true },
				});

			// Назначаем ему роль Customer
			migrationBuilder.InsertData(
				"AspNetUserRoles",
				new[] { "UserId", "RoleId" },
				new object[,]
				{
			{ "1001", "1" }, // Привязка пользователя к роли Customer
			{ "1002", "2" } // Привязка пользователя к роли Manager
				});

			// TODO: добавить заказы и динамику цен
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
