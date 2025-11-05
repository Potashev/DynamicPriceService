using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.Models;

/// <summary>
/// Продукт компании.
/// </summary>
public class Product    //todo: rename to Item?
{
	public int ProductId { get; set; }
	public Company Company { get; set; }
	public int? CompanyId { get; set; }
	public string Title { get; set; }

	[Precision(18, 2)]
	public decimal Price { get; set; }
	public decimal MinimumPrice { get; set; }
	public int? Quantity { get; set; }
	public string? Description { get; set; }

	//todo: remove after adding order entity
	public DateTime? LastSellTime { get; set; }
	public ICollection<PriceDynamic> PriceDynamics { get; set; }
}
