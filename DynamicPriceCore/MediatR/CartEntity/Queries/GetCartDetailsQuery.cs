using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.CartEntity.Queries;

public record GetCartDetailsQuery(int CompanyId) : IRequest<Cart>;