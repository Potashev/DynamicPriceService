namespace DynamicPrice.Core.Models;

public class PriceDynamic
{
	public int Id { get; set; }     //todo: make guid
	public int ProductId { get; set; }
	public decimal Price { get; set; }
	public DateTime? Date { get; set; }
}
