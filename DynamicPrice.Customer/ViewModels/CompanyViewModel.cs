using System.ComponentModel.DataAnnotations;

namespace DynamicPriceClient.ViewModels;

public class CompanyViewModel
{
	public int CompanyId { get; set; }

	[Display(Name = "Company")]
	public string Title { get; set; }
}
