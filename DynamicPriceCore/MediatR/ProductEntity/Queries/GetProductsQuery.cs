using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Queries;

public class GetProductsQuery : IRequest<IEnumerable<ProductViewModel>>
{
	public string UserId { get; set; }
	public GetProductsQuery(string userId) => UserId = userId;
}
