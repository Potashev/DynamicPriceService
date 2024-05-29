using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DynamicPriceService.Models;

/// <summary>
/// Продукт компании.
/// </summary>
public class Product
{
	public int ProductId { get; set; }

    [JsonIgnore]
    public Company Company { get; set; }
	public string Title { get; set; }
	public double Price { get; set; }

	[Display(Name = "Minimum Price")]
	public double MinimumPrice { get; set; }
	public int? Quantity { get; set; }
	public string? Description { get; set; }

    [JsonIgnore]
    //todo: remove after adding order entity
    public DateTime? LastSellTime { get; set; }
}
