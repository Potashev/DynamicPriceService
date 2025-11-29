using DynamicPrice.Core.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.CartEntity.Commands;

public record AddProductToCartCommand(string ProductId) : IRequest<CartViewModel>;