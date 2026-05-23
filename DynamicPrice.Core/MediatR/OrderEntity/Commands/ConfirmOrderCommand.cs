using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public record ConfirmOrderCommand(Guid CartId) : IRequest<Guid>;