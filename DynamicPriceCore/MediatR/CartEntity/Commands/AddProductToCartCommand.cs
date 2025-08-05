using DynamicPriceCore.Models;
using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.CartEntity.Commands;

public record AddProductToCartCommand(string ProductId) : IRequest<CartViewModel>;