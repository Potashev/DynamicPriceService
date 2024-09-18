using DynamicPriceCore.Models;

namespace DynamicPriceCore.MediatR.ViewModels;

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
	//public List<PriceDynamic> PriceDynamics { get; set; } = new List<PriceDynamic>();
	public PriceDynamic[] PriceDynamics { get; set; }

	//public List<decimal> PriceDynamics { get; set; } = new List<decimal>();
}
