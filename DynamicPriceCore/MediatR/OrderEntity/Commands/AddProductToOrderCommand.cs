using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public record AddProductToOrderCommand(string ProductId) : IRequest<Order>;