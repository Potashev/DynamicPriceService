using System.ComponentModel.DataAnnotations;

namespace DynamicPriceService.ViewModels;

public class OrdersStatistics
{
	[Display(Name = "Orders quantity")]
	public int OrdersQuantity { get; set; }

	[Display(Name = "Total amount")]
	public double? TotalAmount { get; set; }

	[Display(Name = "Average order total")]
	public double? AverageOrderTotal { get; set; }
}
