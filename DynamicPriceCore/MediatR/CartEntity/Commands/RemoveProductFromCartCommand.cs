using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.CartEntity.Commands;

public record RemoveProductFromCartCommand(string ProductId) : IRequest<CartViewModel>;