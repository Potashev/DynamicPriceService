using DynamicPriceService.MediatR.ViewModel;
using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Queries;

public class GetProductsQuery : IRequest<IEnumerable<ProductViewModel>>;
