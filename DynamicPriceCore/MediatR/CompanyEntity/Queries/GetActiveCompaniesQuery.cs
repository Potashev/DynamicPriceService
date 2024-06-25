using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public class GetActiveCompaniesQuery : IRequest<IEnumerable<Company>>
{
	//public string UserId { get; set; }
	//public GetProductsQuery(string userId) => UserId = userId;
}
