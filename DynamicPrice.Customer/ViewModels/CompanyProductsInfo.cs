using DynamicPrice.Client.Common.ViewModels;

namespace DynamicPrice.Customer.ViewModels;

public class CompanyProductsInfo
{
	public CompanyViewModel Company { get; set; }
	public ProductInfoViewModel[] Products { get; set; }
}
