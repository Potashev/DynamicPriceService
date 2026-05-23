using DynamicPrice.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Data;

/// <summary>
/// Provides methods for seeding test data into the stores.
/// </summary>
public static class DbInitializer
{
	private const string RoleManager = "Manager";
	private const string RoleCustomer = "Customer";

	public static async Task SeedDataAsync(IServiceProvider services)
	{
		await SeedRolesAsync(services);
		await SeedDomainAsync(services);
		await SeedUsersAsync(services);
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

	public static async Task SeedDomainAsync(IServiceProvider serviceProvider)
	{
		var context = serviceProvider.GetRequiredService<DynamicPriceCoreContext>();

		if (!await context.Companies.AnyAsync())
		{
			context.Companies.AddRange(
				new Company { Title = "Автозапчасти", PriceHistoryLimit = 30 },
				new Company { Title = "Копыта", PriceHistoryLimit = 10 },
				new Company { Title = "Рога", PriceHistoryLimit = 20 }
			);

			await context.SaveChangesAsync();
		}

		var companies = context.Companies.ToList();

		if (!await context.PriceRules.AnyAsync())
		{
			context.PriceRules.AddRange(
				new PriceRule { CompanyId = companies.GetId("Автозапчасти"), Increase = 1, Reduction = 1, NoSellSeconds = 10 },
				new PriceRule { CompanyId = companies.GetId("Копыта"), Increase = 1, Reduction = 1, NoSellSeconds = 10 },
				new PriceRule { CompanyId = companies.GetId("Рога"), Increase = 1, Reduction = 1, NoSellSeconds = 10 }
			);
			await context.SaveChangesAsync();
		}

		if (!await context.Products.AnyAsync())
		{
			var now = DateTime.UtcNow;
			context.Products.AddRange(
				new Product { CompanyId = companies.GetId("Автозапчасти"), Title = "Моторное масло", Price = 5000, MinimumPrice = 1000, Quantity = 100, Description = "Mobil 5w-30", LastSellTime = now },
				new Product { CompanyId = companies.GetId("Автозапчасти"), Title = "Свечи", Price = 1500, MinimumPrice = 1200, Description = "NGK", LastSellTime = now },
				new Product { CompanyId = companies.GetId("Автозапчасти"), Title = "Воздушный фильтр", Price = 700, MinimumPrice = 300, Quantity = 100, LastSellTime = now },
				new Product { CompanyId = companies.GetId("Автозапчасти"), Title = "Ремень ГРМ", Price = 3000, MinimumPrice = 1000, Quantity = 10, LastSellTime = now },
				new Product { CompanyId = companies.GetId("Автозапчасти"), Title = "Антифриз", Price = 500, MinimumPrice = 200, Quantity = 30, LastSellTime = now },
				new Product { CompanyId = companies.GetId("Копыта"), Title = "Хлеб", Price = 50, MinimumPrice = 40, LastSellTime = now },
				new Product { CompanyId = companies.GetId("Копыта"), Title = "Молоко", Price = 100, MinimumPrice = 80, LastSellTime = now }
			);

			//var products = DataGenerator.GenerateProducts(companyId: 1, count: 10);
			//db.Products.AddRange(products);

			await context.SaveChangesAsync();
		}

		//DataGenerator - need for testing metrics on big data

		//var products2 = DataGenerator.GenerateProducts(companyId: 2, count: 100);
		//db.Products.AddRange(products2);
		//var products3 = DataGenerator.GenerateProducts(companyId: 3, count: 1000);
		//db.Products.AddRange(products3);

		await context.SaveChangesAsync();
	}

	private static Guid GetId(this List<Company> companies, string companyTitle)
		=> companies.FirstOrDefault(c => c.Title == companyTitle)
		?.Id ?? Guid.Empty;

	public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
	{
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
		var context = serviceProvider.GetRequiredService<DynamicPriceCoreContext>();

		var companies = context.Companies.ToList();

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

		await EnsureManagerAsync(userManager,
			email: "manager@test.com",
			userName: "Man1",
			companyId: companies.GetId("Автозапчасти"));

		await EnsureManagerAsync(userManager,
			email: "manager2@test.com",
			userName: "Man2",
			companyId: companies.GetId("Копыта"));
	}

	private static async Task EnsureManagerAsync(
		UserManager<ApplicationUser> userManager,
		string email,
		string userName,
		Guid companyId)
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

		if (user.CompanyId != companyId)
		{
			user.CompanyId = companyId;
			await userManager.UpdateAsync(user);
		}

		if (!await userManager.IsInRoleAsync(user, RoleManager))
			await userManager.AddToRoleAsync(user, RoleManager);
	}
}
