using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.CartEntity.Commands;

public record RemoveProductFromCartCommand(string ProductId) : IRequest<Cart>;