using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public record DeleteProductCommand(int ProductId) : IRequest;
