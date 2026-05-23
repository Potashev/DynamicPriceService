using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public record MakeActiveProductCommand(Guid ProductId) : IRequest;
