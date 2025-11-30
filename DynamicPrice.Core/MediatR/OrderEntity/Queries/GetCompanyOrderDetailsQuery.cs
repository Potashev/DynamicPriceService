using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public record GetCompanyOrderDetailsQuery(string OrderId) : IRequest<OrderViewModel>;