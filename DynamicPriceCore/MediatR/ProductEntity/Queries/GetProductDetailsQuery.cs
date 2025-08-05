using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Queries;

public record GetProductDetailsQuery(int ProductId) : IRequest<ProductViewModel>;
