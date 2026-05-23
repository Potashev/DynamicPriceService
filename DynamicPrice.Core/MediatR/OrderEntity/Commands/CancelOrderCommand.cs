using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public record CancelOrderCommand(Guid OrderId) : IRequest<Guid>;
