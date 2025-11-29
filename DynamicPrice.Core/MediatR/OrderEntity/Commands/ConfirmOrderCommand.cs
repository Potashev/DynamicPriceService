using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public record ConfirmOrderCommand(int CartId) : IRequest<int>;