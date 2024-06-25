using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class CompleteOrderCommand : IRequest<int>
{
	public string OrderId { get; set; }
	public CompleteOrderCommand(string orderId)
		=> OrderId = orderId;
}
