using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Queries;

public record GetProductsQuery() : IRequest<IEnumerable<ProductViewModel>>;
