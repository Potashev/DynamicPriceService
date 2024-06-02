using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleDetailsQuery : IRequest<PriceRuleViewModel>
{
	public string UserId { get; set; }
	public GetPriceRuleDetailsQuery(string userId) => UserId = userId;
}
