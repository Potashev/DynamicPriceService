using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.CartEntity.Commands;

public record AddProductToCartCommand(string ProductId) : IRequest<Cart>;