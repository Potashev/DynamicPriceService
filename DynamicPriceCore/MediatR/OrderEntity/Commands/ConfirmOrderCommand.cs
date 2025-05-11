using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public record ConfirmOrderCommand(int CartOrderId) : IRequest<int>;