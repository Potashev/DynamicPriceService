using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;

namespace DynamicPrice.Core.MediatR.CartEntity.Commands;

public record AddProductToCartCommand(string ProductId) : IRequest<CartViewModel>;