using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public record GetCompanyOrdersQuery(string UserId) : IRequest<IEnumerable<OrderViewModel>>;