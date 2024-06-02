using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Queries;

public class GetProductDetailsQuery : IRequest<ProductViewModel>
{
	public int ProductId { get; set; }

	public GetProductDetailsQuery(int productId) => ProductId = productId;
}
