using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Queries;

public record GetProductDetailsQuery(Guid ProductId) : IRequest<ProductViewModel>;
