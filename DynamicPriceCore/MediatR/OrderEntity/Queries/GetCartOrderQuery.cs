using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCartOrderQuery : IRequest<Order>
{
	public int CustomerId { get; set; }
	public int CompanyId { get; set; }

	public GetCartOrderQuery(int customerId, int companyId) => (CustomerId, CompanyId) = (customerId, companyId);
}
