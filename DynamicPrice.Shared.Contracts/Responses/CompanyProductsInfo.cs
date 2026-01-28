namespace DynamicPrice.Shared.Contracts.ViewModels.Responses;

public class CompanyProductsInfo
{
	public required CompanyViewModel Company { get; init; }
	public ICollection<ProductInfoViewModel> Products { get; init; } = [];
}
