using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DynamicPriceCore.Services;

public class CurrentUserService : ICurrentUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly UserManager<ApplicationUser> _userManager;
	//private readonly DynamicPriceCoreContext _context;

	public CurrentUserService(
		IHttpContextAccessor httpContextAccessor,
		UserManager<ApplicationUser> userManager,
		DynamicPriceCoreContext context)
	{
		_httpContextAccessor = httpContextAccessor;
		_userManager = userManager;
		//_context = context;
	}

	public string? UserId =>
		_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

	public string? Role =>
		_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

	//public async Task<Manager?> GetCurrentManagerAsync()
	//{
	//	if (Role != "Manager" || string.IsNullOrEmpty(UserId))
	//		return null;

	//	return await _context.Managers
	//		.Include(m => m.Company)
	//		.FirstOrDefaultAsync(m => m.Id == UserId);
	//}

	//public async Task<Customer?> GetCurrentCustomerAsync()
	//{
	//	if (Role != "Customer" || string.IsNullOrEmpty(UserId))
	//		return null;

	//	return await _context.Customers
	//		.FirstOrDefaultAsync(m => m.Id == UserId);
	//}

	public async Task<ApplicationUser?> GetCurrentUserAsync()
	{
		//todo: check
		//return await _context.Users
		//	.Include(m => m.Company)
		//	.FirstOrDefaultAsync(m => m.Id == UserId);

		//temp
		return await _userManager.FindByIdAsync(UserId ?? string.Empty);
	}
}

public interface ICurrentUserService
{
	string? UserId { get; }
	string? Role { get; }
	Task<ApplicationUser?> GetCurrentUserAsync();
	//Task<Manager?> GetCurrentManagerAsync();
	//Task<Customer?> GetCurrentCustomerAsync();
}
