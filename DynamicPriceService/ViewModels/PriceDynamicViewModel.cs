using System.Text.Json.Serialization;

namespace DynamicPriceService.ViewModels;

public class PriceDynamicViewModel
{
	public int Id { get; set; }

	[JsonIgnore]
	public ProductViewModel Product { get; set; }
	public decimal Price { get; set; }
	public DateTime? Date { get; set; }
}
