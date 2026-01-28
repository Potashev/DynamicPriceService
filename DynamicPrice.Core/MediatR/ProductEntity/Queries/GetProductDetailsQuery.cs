using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Queries;

public record GetProductDetailsQuery(int ProductId) : IRequest<ProductViewModel>;
