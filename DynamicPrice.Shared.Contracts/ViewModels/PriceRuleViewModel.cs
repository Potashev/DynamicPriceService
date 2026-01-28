namespace DynamicPrice.Shared.Contracts.ViewModels;

public class PriceRuleViewModel
{
	public int PriceRuleId { get; init; }
	public double Increase { get; init; }
	public double Reduction { get; init; }
	public TimeSpan NoSellTime { get; init; }
}
