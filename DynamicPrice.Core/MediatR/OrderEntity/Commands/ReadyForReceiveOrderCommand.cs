using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public record ReadyForReceiveOrderCommand(Guid OrderId) : IRequest;
