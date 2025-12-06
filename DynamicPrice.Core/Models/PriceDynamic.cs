namespace DynamicPrice.Core.Models;

public class PriceDynamic
{
	public int Id { get; set; }     //todo: make guid

	//[JsonIgnore]
	//public Product Product { get; set; }
	public int ProductId { get; set; }
	public decimal Price { get; set; }
	public DateTime? Date { get; set; }
}
