using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public record CreateProductCommand(ProductViewModel ProductVm) : IRequest<int>;
