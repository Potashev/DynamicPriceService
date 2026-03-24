using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.ViewModels;

public class PriceRuleViewModel
{
	public int PriceRuleId { get; init; }

	//[DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
	public double Increase { get; init; }

	//[DataType(DataType.)]
	public double Reduction { get; init; }
	public TimeSpan NoSellTime { get; init; }
}
