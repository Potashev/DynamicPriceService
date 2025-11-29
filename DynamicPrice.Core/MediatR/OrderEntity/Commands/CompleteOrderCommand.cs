using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public record CompleteOrderCommand(string OrderId) : IRequest<int>;