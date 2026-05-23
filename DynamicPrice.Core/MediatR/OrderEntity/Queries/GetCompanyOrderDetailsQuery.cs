using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public record GetCompanyOrderDetailsQuery(Guid OrderId) : IRequest<OrderViewModel>;