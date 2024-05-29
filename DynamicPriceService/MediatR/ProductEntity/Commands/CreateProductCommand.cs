using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Commands;

public class CreateProductCommand : IRequest
{
	public Product Product { get; set; }
	public CreateProductCommand(Product product) => Product = product;
}
