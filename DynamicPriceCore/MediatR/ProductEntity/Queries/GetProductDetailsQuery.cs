using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Queries;

public class GetProductDetailsQuery : IRequest<Product>
{
	public int ProductId { get; set; }

	public GetProductDetailsQuery(int productId) => ProductId = productId;
}
