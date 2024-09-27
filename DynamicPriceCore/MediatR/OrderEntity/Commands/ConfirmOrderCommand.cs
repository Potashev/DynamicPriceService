using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public record ConfirmOrderCommand(int CustomerId, int CartOrderId) : IRequest<int>;