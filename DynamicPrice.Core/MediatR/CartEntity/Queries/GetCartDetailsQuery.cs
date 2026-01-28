using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;

namespace DynamicPrice.Core.MediatR.CartEntity.Queries;

public record GetCartDetailsQuery(int CompanyId) : IRequest<CartViewModel>;