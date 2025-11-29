using System.ComponentModel.DataAnnotations;

namespace DynamicPriceClient.ViewModels;

public class OrderItemViewModel
{
	public int Id { get; set; }
	public ProductViewModel Product { get; set; }

	[Display(Name = "Price")]
	public decimal? ProductPrice { get; set; }
	public int Quantity { get; set; }
}
