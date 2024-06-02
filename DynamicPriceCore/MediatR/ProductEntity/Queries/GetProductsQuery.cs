using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Queries;

public class GetProductsQuery : IRequest<IEnumerable<Product>>
{
    public string UserId { get; set; }
    public GetProductsQuery(string userId) => UserId = userId;
}
