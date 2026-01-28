using System.Text.Json.Serialization;

namespace DynamicPrice.Shared.Contracts.ViewModels.Entities;

public class CartItemViewModel
{
	public int Id { get; set; }
	public int CartId { get; set; }

	[JsonIgnore]
	public CartViewModel Cart { get; set; }
	public int ProductId { get; set; }
	public ProductViewModel Product { get; set; }
	public int Quantity { get; set; }
}
