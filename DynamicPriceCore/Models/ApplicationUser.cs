using Microsoft.AspNetCore.Identity;

namespace DynamicPriceCore.Models;

public class ApplicationUser : IdentityUser
{
	public decimal Balance { get; set; }
	public int? CompanyId { get; set; }

	public Company? Company { get; set; } // если есть связь с Company
}
