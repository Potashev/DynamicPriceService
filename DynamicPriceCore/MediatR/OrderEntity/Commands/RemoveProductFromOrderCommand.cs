using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public record RemoveProductFromOrderCommand(string CustomerId, string ProductId) : IRequest<Order>;