using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public class GetCompanyProductsQuery : IRequest<CompanyProductsInfo>
{
	public string CompanyId { get; set; }
	public GetCompanyProductsQuery(string companyId) => CompanyId = companyId;
}
