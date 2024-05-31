using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleDetailsQuery : IRequest<PriceRule>
{
	public string UserId { get; set; }
	public GetPriceRuleDetailsQuery(string userId) => UserId = userId;
}
