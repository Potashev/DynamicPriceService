using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public record GetCompanyOrdersQuery() : IRequest<IEnumerable<OrderViewModel>>;