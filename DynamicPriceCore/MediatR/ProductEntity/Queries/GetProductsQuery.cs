using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Queries;

public record GetProductsQuery(string UserId) : IRequest<IEnumerable<ProductViewModel>>;
