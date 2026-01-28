using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public record GetCompanyOrderDetailsQuery(string OrderId) : IRequest<OrderViewModel>;