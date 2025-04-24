using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public record CreateProductCommand(ProductViewModel ProductVm) : IRequest<int>;
