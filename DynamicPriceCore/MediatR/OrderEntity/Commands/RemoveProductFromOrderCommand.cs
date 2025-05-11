using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public record RemoveProductFromOrderCommand(string ProductId) : IRequest<Order>;