using System.Text.Json.Serialization;

namespace DynamicPriceCore.Models;

public class OrderItem
{
	public int Id { get; set; }     //todo: make guid
	public int OrderId { get; set; }

	[JsonIgnore]
	public Order Order { get; set; }
	public int ProductId { get; set; }
	public Product Product { get; set; }
	public decimal ProductPrice { get; set; }
	public int Quantity { get; set; }
}
