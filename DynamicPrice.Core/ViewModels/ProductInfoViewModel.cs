using DynamicPrice.Core.Models;

namespace DynamicPrice.Core.ViewModels;

/// <summary>
/// Dto продукта для покупателя.
/// </summary>
public class ProductInfoViewModel
{
	public int ProductId { get; set; }
	public string Title { get; set; }
	public decimal Price { get; set; }
	public int? Quantity { get; set; }
	public string? Description { get; set; }
	public PriceDynamic[] PriceDynamics { get; set; }
}
