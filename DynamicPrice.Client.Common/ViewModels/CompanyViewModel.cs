using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Client.Common.ViewModels;

public class CompanyViewModel
{
	public int CompanyId { get; set; }

	[Display(Name = "Company")]
	public string Title { get; set; }
}
