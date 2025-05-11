using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public record GetCartOrderQuery(int CompanyId) : IRequest<Order>;