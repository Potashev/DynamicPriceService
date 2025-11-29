using Microsoft.AspNetCore.Identity;

namespace DynamicPrice.Core.Models;

public class ApplicationUser : IdentityUser
{
	public decimal Balance { get; set; }
	public int? CompanyId { get; set; }
}
