using DynamicPriceCore.ViewModels;

namespace DynamicPrice.Core.ViewModels;

public class ManagerInfoViewModel
{
	public string Name { get; set; }
	public string Email { get; set; }
	public CompanyViewModel Company { get; set; }
}
