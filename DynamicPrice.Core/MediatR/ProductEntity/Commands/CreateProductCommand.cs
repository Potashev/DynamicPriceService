using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public record CreateProductCommand(ProductViewModel ProductVm) : IRequest<Guid>;
