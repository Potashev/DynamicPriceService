using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCartOrderQuery : IRequest<Order>
{
	public int CustomerId { get; set; }

	public GetCartOrderQuery(int customerId) => CustomerId = customerId;
}
