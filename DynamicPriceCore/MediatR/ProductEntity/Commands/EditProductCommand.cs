using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public record EditProductCommand(ProductViewModel ProductVm) : IRequest<int>;
