namespace DynamicPriceService.ViewModels;

public class PriceRuleViewModel
{
	public int PriceRuleId { get; set; }
	public double Increase { get; set; }
	public double Reduction { get; set; }
	public TimeSpan? NoSellTime { get; set; }
}
