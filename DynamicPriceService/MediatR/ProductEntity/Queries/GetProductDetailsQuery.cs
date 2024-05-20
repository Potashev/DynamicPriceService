using DynamicPriceService.MediatR.ViewModel;
using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Queries;

public class GetProductDetailsQuery : IRequest<ProductViewModel>
{
	public int ProductId { get; set; }

	public GetProductDetailsQuery(int productId) => ProductId = productId;
}
