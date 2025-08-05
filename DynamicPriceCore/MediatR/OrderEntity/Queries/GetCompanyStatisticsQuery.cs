using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public record GetCompanyStatisticsQuery() : IRequest<OrderStatistics>;