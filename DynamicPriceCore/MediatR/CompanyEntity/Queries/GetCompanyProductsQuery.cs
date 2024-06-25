using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public class GetCompanyProductsQuery : IRequest<IEnumerable<ProductInfoViewModel>>
{
	public string CompanyId { get; set; }
	public GetCompanyProductsQuery(string companyId) => CompanyId = companyId;
}
