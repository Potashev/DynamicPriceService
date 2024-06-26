using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class AddProductToOrderCommand : IRequest<int>
{
	public string CustomerId { get; set; }
	public string ProductId { get; set; }
	public AddProductToOrderCommand(string customerId, string productId)
		=> (CustomerId, ProductId) = (customerId, productId);
}
