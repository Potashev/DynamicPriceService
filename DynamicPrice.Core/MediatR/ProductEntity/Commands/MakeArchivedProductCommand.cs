using MediatR;

namespace DynamicPrice.Core.MediatR.ProductEntity.Commands;

public record MakeArchivedProductCommand(int ProductId) : IRequest;
