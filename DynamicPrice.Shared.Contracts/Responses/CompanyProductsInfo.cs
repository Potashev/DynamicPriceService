namespace DynamicPrice.Shared.Contracts.ViewModels.Responses;

public class CompanyProductsInfo
{
	public CompanyViewModel Company { get; set; }
	public ProductInfoViewModel[] Products { get; set; }
}
