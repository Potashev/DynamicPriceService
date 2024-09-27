using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public record AddProductToOrderCommand(string CustomerId, string ProductId) : IRequest<Order>;