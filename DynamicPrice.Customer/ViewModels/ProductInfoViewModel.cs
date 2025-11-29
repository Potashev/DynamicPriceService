using DynamicPrice.Client.Common.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Customer.ViewModels;

/// <summary>
/// Dto продукта для покупателя.
/// </summary>
public class ProductInfoViewModel
{
	public int ProductId { get; set; }

	[Display(Name = "Product")]
	public string Title { get; set; }
	public decimal Price { get; set; }
	public int? Quantity { get; set; }
	public string? Description { get; set; }

	[Display(Name = "Price dynamics")]
	public PriceDynamicViewModel[] PriceDynamics { get; set; }
}
