using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class CreateProductCommand : IRequest<int>
{
	public Product Product { get; set; }
    public string UserId { get; set; }
    public CreateProductCommand(Product product, string userId) 
        => (Product, UserId) = (product, userId);
}
