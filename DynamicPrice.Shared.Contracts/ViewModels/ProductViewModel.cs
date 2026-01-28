using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.ViewModels;

public class ProductViewModel
{
	[Required]   // todo: check
	public int ProductId { get; set; }
	public string Title { get; set; }
	public decimal Price { get; set; }
	public decimal MinimumPrice { get; set; }
	public int? Quantity { get; set; }
	public string? Description { get; set; }
	public PriceDynamicViewModel[]? PriceDynamics { get; set; }
}
