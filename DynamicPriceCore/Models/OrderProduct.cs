﻿using System.Text.Json.Serialization;

namespace DynamicPriceCore.Models;

public class OrderProduct
{
	public int Id { get; set; }
	[JsonIgnore]
	public Order Order { get; set; }
	public int OrderId { get; set; }
	public Product Product { get; set; }
	public int ProductId { get; set; }
	public double? Price { get; set; }
	public int Quantity { get; set; }
}