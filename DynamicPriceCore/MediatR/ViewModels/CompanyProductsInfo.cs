namespace DynamicPriceCore.MediatR.ViewModels;
public class CompanyProductsInfo
{
	public string CompanyId { get; set; }
	public ProductInfoViewModel[] Products { get; set; }
    public CompanyProductsInfo(string companyId, ProductInfoViewModel[] ProductsInfoVm) 
    {
        CompanyId = companyId;
		Products = ProductsInfoVm;
	}

}
