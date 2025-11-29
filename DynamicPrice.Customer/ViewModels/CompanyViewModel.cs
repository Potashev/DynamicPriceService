using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Customer.ViewModels;

public class CompanyViewModel
{
	public int CompanyId { get; set; }

	[Display(Name = "Company")]
	public string Title { get; set; }
}
