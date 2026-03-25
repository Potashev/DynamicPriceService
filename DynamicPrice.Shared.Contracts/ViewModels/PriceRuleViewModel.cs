using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.ViewModels;

public class PriceRuleViewModel
{
	public int PriceRuleId { get; init; }

	[Range(0, double.MaxValue, ErrorMessage = "Increase cannot be negative")]
	public double Increase { get; init; }

	[Range(0, double.MaxValue, ErrorMessage = "Reduction cannot be negative")]
	public double Reduction { get; init; }
	public TimeSpan NoSellTime { get; init; }
}
