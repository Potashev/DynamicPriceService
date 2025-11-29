using DynamicPrice.Core.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.CartEntity.Commands;

public record RemoveProductFromCartCommand(string ProductId) : IRequest<CartViewModel>;