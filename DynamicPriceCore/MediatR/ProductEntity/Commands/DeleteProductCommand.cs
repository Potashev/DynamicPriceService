using MediatR;

namespace DynamicPriceCore.MediatR.ProductEntity.Commands;

public record DeleteProductCommand(int ProductId) : IRequest;
