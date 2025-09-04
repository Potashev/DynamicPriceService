using System.ComponentModel.DataAnnotations;

namespace DynamicPriceClient.ViewModels;

public class ProductViewModel
{
	public int ProductId { get; set; }

	[Display(Name = "Product")]
	public string Title { get; set; }
	public double Price { get; set; }
	public double MinimumPrice { get; set; }
	public int? Quantity { get; set; }
	public string? Description { get; set; }
}
