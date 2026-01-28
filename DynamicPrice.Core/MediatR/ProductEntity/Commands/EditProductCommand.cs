using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public record EditProductCommand(ProductViewModel ProductVm) : IRequest<int>;
