using DynamicPriceCore.Models;

namespace DynamicPriceClient.ViewModels;

public class PriceDynamicViewModel
{
	public int Id { get; set; }
	//public Product Product { get; set; }
	public int ProductId { get; set; }
	public decimal Price { get; set; }
	public DateTime? Date { get; set; }
}
