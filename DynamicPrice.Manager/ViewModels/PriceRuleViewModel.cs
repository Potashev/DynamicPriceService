using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Manager.ViewModels;

public class PriceRuleViewModel
{
	public int PriceRuleId { get; set; }
	public double Increase { get; set; }
	public double Reduction { get; set; }

	[Display(Name = "No sell time")]
	public TimeSpan? NoSellTime { get; set; }
}
