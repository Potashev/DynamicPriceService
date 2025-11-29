using DynamicPrice.Core.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public record CreateProductCommand(ProductViewModel ProductVm) : IRequest<int>;
