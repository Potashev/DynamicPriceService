using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class EditProductCommand : IRequest<int>
{
	public Product Product { get; set; }
	public EditProductCommand(Product product) => Product = product;
}
