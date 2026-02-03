namespace DynamicPrice.Shared.Contracts.ViewModels.Responses;

public class PriceRuleWithStatus
{
	public required PriceRuleViewModel PriceRule { get; init; }
	public bool IsActive { get; init; }
}
