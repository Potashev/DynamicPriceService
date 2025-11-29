using DynamicPrice.Core.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public record EditProductCommand(ProductViewModel ProductVm) : IRequest<int>;
