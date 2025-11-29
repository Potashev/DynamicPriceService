namespace DynamicPrice.Core.ViewModels;

public class CompanyProductsInfo
{
	public CompanyViewModel Company { get; set; }
	public ProductInfoViewModel[] Products { get; set; }
	public CompanyProductsInfo(CompanyViewModel companyVm, ProductInfoViewModel[] ProductsInfoVm)
	{
		Company = companyVm;
		Products = ProductsInfoVm;
	}

}
