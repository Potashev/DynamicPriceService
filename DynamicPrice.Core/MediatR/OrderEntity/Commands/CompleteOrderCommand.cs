using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public record CompleteOrderCommand(string OrderId) : IRequest<int>;