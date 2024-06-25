using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCompanyOrderDetailsQuery : IRequest<OrderViewModel>
{
	public string OrderId { get; set; }
	public GetCompanyOrderDetailsQuery(string orderId) => OrderId = orderId;
}
