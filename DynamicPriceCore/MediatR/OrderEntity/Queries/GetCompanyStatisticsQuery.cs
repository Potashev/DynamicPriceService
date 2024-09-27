using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public record GetCompanyStatisticsQuery(string UserId) : IRequest<OrderStatistics>;