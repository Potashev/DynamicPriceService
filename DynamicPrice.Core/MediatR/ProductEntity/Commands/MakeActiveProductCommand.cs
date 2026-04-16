using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public record MakeActiveProductCommand(int ProductId) : IRequest;
