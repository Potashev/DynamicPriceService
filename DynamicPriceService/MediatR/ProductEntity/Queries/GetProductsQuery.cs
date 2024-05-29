using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.ProductEntity.Queries;

public class GetProductsQuery : IRequest<IEnumerable<Product>>;
