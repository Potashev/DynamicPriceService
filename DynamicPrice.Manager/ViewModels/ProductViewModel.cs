using DynamicPriceService.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Manager.ViewModels;

public class ProductViewModel
{
	public int ProductId { get; set; }
	public string Title { get; set; }
	public decimal Price { get; set; }


	[Display(Name = "Minimum price")]
	public double MinimumPrice { get; set; }
	public int? Quantity { get; set; }
	public string? Description { get; set; }

	[Display(Name = "Price dynamics")]
	public PriceDynamicViewModel[] PriceDynamics { get; set; }
}
