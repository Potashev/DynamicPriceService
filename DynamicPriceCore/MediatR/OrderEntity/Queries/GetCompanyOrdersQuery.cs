using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public record GetCompanyOrdersQuery() : IRequest<IEnumerable<OrderViewModel>>;