using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Queries;

public record GetPriceRuleWithStatusQuery() : IRequest<PriceRuleWithStatus>;

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
