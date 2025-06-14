using Microsoft.AspNetCore.Identity;

namespace DynamicPriceCore.Models;

//todo: think about this instead Manager and Customer
public class ApplicationUser : IdentityUser
{
	public decimal Balance { get; set; }
	public int? CompanyId { get; set; }

	public Company? Company { get; set; }
}
