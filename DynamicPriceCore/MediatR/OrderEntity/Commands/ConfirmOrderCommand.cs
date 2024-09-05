using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class ConfirmOrderCommand : IRequest<int>
{
	public int CustomerId { get; set; }
	public int CastOrderId { get; set; }
	public ConfirmOrderCommand(int customerId, int cartOrderId)
		=> (CustomerId, CastOrderId) = (customerId, cartOrderId);
}
