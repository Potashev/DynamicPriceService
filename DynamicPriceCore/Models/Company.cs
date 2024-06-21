namespace DynamicPriceCore.Models;

public class Company
{
	public int CompanyId { get; set; }
	public string Title { get; set; }
	public ICollection<CompanyUser> CompanyUsers { get; set; }
}
