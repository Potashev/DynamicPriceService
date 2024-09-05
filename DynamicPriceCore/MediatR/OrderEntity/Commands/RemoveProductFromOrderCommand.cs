using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class RemoveProductFromOrderCommand : IRequest
{
	public string CustomerId { get; set; }
	public string ProductId { get; set; }
	public RemoveProductFromOrderCommand(string customerId, string productId)
		=> (CustomerId, ProductId) = (customerId, productId);
}
