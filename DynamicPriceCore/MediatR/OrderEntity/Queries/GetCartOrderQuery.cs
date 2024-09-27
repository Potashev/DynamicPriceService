using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public record GetCartOrderQuery(int CustomerId, int CompanyId) : IRequest<Order>;