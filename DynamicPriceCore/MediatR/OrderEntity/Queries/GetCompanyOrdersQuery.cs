using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCompanyOrdersQuery : IRequest<IEnumerable<OrderViewModel>>
{
	public string UserId { get; set; }
	public GetCompanyOrdersQuery(string userId) => UserId = userId;
}
