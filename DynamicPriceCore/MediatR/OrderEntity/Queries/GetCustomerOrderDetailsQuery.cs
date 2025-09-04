using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public record GetCustomerOrderDetailsQuery(string OrderId) : IRequest<OrderInfoViewModel>;