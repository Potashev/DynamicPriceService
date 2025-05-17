using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicPriceCore.Data;

/// <summary>
/// Provides methods for seeding test users data into the database.
/// </summary>
public static class DbInitializer
{
	public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		string[] roleNames = { "Manager", "Customer" };

		foreach (var roleName in roleNames)
		{
			if (!await roleManager.RoleExistsAsync(roleName))
			{
				await roleManager.CreateAsync(new IdentityRole(roleName));
			}
		}
	}

	public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
	{
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		// Customer
		var customerEmail = "customer@test.com";
		if (await userManager.FindByEmailAsync(customerEmail) is null)
		{
			var customer = new ApplicationUser
			{
				UserName = "Nik1",
				Email = customerEmail,
				EmailConfirmed = true,
				Balance = 100,

				
			};

			var result = await userManager.CreateAsync(customer, "sfjk23Q/+");
			if (result.Succeeded)
				await userManager.AddToRoleAsync(customer, "Customer");
		}

		// Manager
		var managerEmail = "manager@test.com";
		if (await userManager.FindByEmailAsync(managerEmail) is null)
		{
			var manager = new ApplicationUser
			{
				UserName = "Man1",
				Email = managerEmail,
				EmailConfirmed = true,
				CompanyId = 1
			};

			var result = await userManager.CreateAsync(manager, "sfjk23Q/+");
			if (result.Succeeded)
				await userManager.AddToRoleAsync(manager, "Manager");
		}
	}

}
