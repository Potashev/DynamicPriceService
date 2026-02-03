using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public record GetCompanyStatisticsQuery() : IRequest<OrdersStatistics>;