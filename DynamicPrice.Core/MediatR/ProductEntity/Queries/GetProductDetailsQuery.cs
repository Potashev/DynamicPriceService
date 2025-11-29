using DynamicPrice.Core.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Queries;

public record GetProductDetailsQuery(int ProductId) : IRequest<ProductViewModel>;
