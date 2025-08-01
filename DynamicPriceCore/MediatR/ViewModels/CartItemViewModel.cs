using System.Text.Json.Serialization;

namespace DynamicPriceCore.MediatR.ViewModels;

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
