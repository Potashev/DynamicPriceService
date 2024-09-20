using System.Text.Json.Serialization;

namespace DynamicPriceCore.Models;

public class PriceDynamic
{
	public int Id { get; set; }
	[JsonIgnore]
	public Product Product { get; set; }
	public decimal Price { get; set; }
	public DateTime? Date { get; set; }
}
