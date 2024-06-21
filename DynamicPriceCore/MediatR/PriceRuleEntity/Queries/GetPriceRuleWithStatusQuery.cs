using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleWithStatusQuery : IRequest<PriceRuleWithStatus>
{
	public string UserId { get; set; }
	public GetPriceRuleWithStatusQuery(string userId) => UserId = userId;
}

public class PriceRuleWithStatus
{
	public PriceRuleViewModel PriceRuleVm { get; set; }
	public bool IsActive { get; set; }
	public PriceRuleWithStatus(PriceRuleViewModel priceRuleVm, bool isActive)
	{
		PriceRuleVm = priceRuleVm;
		IsActive = isActive;
	}
}
