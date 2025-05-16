using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicPriceCore.Data;

/// <summary>
/// Provides methods for seeding test users data into the database.
/// </summary>
public static class DbInitializer
{
	public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
	{
		var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

		// Проверка, есть ли уже такой пользователь
		var user = await userManager.FindByEmailAsync("test@example.com");
		if (user == null)
		{
			var newUser = new Customer
			{
				UserName = "Nik1",
				Email = "test@example.com",
				EmailConfirmed = true,
				Balance = 10
			};

			var result = await userManager.CreateAsync(newUser, "sfjk23Q/+");
			if (!result.Succeeded)
			{
				throw new Exception("Не удалось создать тестового пользователя: " +
					string.Join(", ", result.Errors.Select(e => e.Description)));
			}
		}
	}
}
