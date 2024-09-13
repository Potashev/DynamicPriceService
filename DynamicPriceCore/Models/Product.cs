using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DynamicPriceCore.Models;

/// <summary>
/// Продукт компании.
/// </summary>
public class Product
{
	public int ProductId { get; set; }
	public Company Company { get; set; }
	public string Title { get; set; }

	[Precision(18, 2)]
	public decimal Price { get; set; }

	public decimal MinimumPrice { get; set; }
	public int? Quantity { get; set; }
	public string? Description { get; set; }

	//todo: remove after adding order entity
	public DateTime? LastSellTime { get; set; }
	[JsonIgnore]
	public ICollection<OrderProduct> OrderProducts { get; set; }
}
