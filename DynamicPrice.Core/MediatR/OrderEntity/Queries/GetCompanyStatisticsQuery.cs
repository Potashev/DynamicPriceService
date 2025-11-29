using DynamicPrice.Core.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public record GetCompanyStatisticsQuery() : IRequest<OrdersStatistics>;