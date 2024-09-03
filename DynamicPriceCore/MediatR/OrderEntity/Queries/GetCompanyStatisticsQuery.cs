using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetCompanyStatisticsQuery : IRequest<OrderStatistics>
{
	public string UserId { get; set; }
	public GetCompanyStatisticsQuery(string userId) => UserId = userId;
}
