using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicPriceCore.Data;

/// <summary>
/// Provides methods for seeding test users data into the database.
/// </summary>
public static class DbInitializer
{
	private const string RoleManager = "Manager";
	private const string RoleCustomer = "Customer";

	public static async Task SeedAllAsync(IServiceProvider services)
	{
		// порядок важен
		await SeedRolesAsync(services);
		await SeedDomainAsync(services);   // компании/товары/правила
		await SeedUsersAsync(services);    // пользователи + роли + привязка к компаниям
	}

	public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		foreach (var roleName in new[] { RoleManager, RoleCustomer })
		{
			if (!await roleManager.RoleExistsAsync(roleName))
				await roleManager.CreateAsync(new IdentityRole(roleName));
		}
	}

	public static async Task SeedDomainAsync(IServiceProvider sp)
	{
		var db = sp.GetRequiredService<DynamicPriceCoreContext>();

		//var companies = new Company[]
		//{
		//	new Company { Title = "Автозапчасти" },
		//	new Company { Title = "Копыта" }
		//};

		// Компании (идемпотентно)
		if (!await db.Companies.AnyAsync())
		{
			db.Companies.AddRange(
				new Company { Title = "Автозапчасти" },
				new Company { Title = "Копыта" }
			);

			//db.Companies.AddRange(companies);

			await db.SaveChangesAsync();
		}

		// Правила
		if (!await db.PriceRules.AnyAsync())
		{
			db.PriceRules.AddRange(
				new PriceRule { CompanyId = 1, Increase = 1, Reduction = 1, NoSellTime = TimeSpan.FromSeconds(10) },
				new PriceRule { CompanyId = 2, Increase = 1, Reduction = 1, NoSellTime = TimeSpan.FromSeconds(10) }
			);
			await db.SaveChangesAsync();
		}

		// Товары
		if (!await db.Products.AnyAsync())
		{
			var now = DateTime.UtcNow;
			db.Products.AddRange(
				new Product { CompanyId = 1, Title = "Моторное масло", Price = 5000, MinimumPrice = 1000, Quantity = 100, Description = "Mobil 5w-30", LastSellTime = now },
				new Product { CompanyId = 1, Title = "Свечи", Price = 1500, MinimumPrice = 1200, Description = "NGK", LastSellTime = now },
				new Product { CompanyId = 1, Title = "Воздушный фильтр", Price = 700, MinimumPrice = 300, Quantity = 100, LastSellTime = now },
				new Product { CompanyId = 1, Title = "Ремень ГРМ", Price = 3000, MinimumPrice = 1000, Quantity = 10, LastSellTime = now },
				new Product { CompanyId = 1, Title = "Антифриз", Price = 500, MinimumPrice = 200, Quantity = 30, LastSellTime = now },
				new Product { CompanyId = 2, Title = "Хлеб", Price = 50, MinimumPrice = 40, LastSellTime = now },
				new Product { CompanyId = 2, Title = "Молоко", Price = 100, MinimumPrice = 80, LastSellTime = now }
			);
			await db.SaveChangesAsync();
		}
	}

	public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
	{
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		// Customer
		var customerEmail = "customer@test.com";
		var customer = await userManager.FindByEmailAsync(customerEmail);
		if (customer is null)
		{
			customer = new ApplicationUser
			{
				UserName = "Nik1",
				Email = customerEmail,
				EmailConfirmed = true,
				Balance = 100m
			};

			var result = await userManager.CreateAsync(customer, "sfjk23Q/+");
			if (!result.Succeeded)
				throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
		}
		if (!await userManager.IsInRoleAsync(customer, RoleCustomer))
			await userManager.AddToRoleAsync(customer, RoleCustomer);

		// Manager 1 (CompanyId = 1)
		await EnsureManagerAsync(userManager,
			email: "manager@test.com",
			userName: "Man1",
			companyId: 1);

		// Manager 2 (CompanyId = 2)
		await EnsureManagerAsync(userManager,
			email: "manager2@test.com",
			userName: "Man2",
			companyId: 2);
	}

	private static async Task EnsureManagerAsync(UserManager<ApplicationUser> userManager, string email, string userName, int companyId)
	{
		var user = await userManager.FindByEmailAsync(email);
		if (user is null)
		{
			user = new ApplicationUser
			{
				UserName = userName,
				Email = email,
				EmailConfirmed = true,
				CompanyId = companyId
			};

			var result = await userManager.CreateAsync(user, "sfjk23Q/+");
			if (!result.Succeeded)
				throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
		}

		// Если у существующего менеджера ещё не установлен CompanyId — проставим
		if (user.CompanyId != companyId)
		{
			user.CompanyId = companyId;
			await userManager.UpdateAsync(user);
		}

		if (!await userManager.IsInRoleAsync(user, RoleManager))
			await userManager.AddToRoleAsync(user, RoleManager);
	}
}
