using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public record GetCompanyOrderDetailsQuery(string OrderId) : IRequest<OrderViewModel>;