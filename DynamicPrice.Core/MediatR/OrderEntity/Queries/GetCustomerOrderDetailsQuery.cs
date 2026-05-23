using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public record GetCustomerOrderDetailsQuery(Guid OrderId) : IRequest<OrderViewModel>;