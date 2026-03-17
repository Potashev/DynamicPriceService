using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public record CancelOrderCommand(int OrderId) : IRequest<int>;
