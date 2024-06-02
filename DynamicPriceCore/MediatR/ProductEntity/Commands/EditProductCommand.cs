using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public class EditProductCommand : IRequest<int>
{
	public Product Product { get; set; }
	public EditProductCommand(Product product) => Product = product;
}
