using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.CartEntity.Queries;

public record GetCartDetailsQuery(int CompanyId) : IRequest<CartViewModel>;