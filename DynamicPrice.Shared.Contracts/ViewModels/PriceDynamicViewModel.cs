namespace DynamicPrice.Shared.Contracts.ViewModels;

public class PriceDynamicViewModel
{
	public int Id { get; set; }
	public int ProductId { get; set; }
	public decimal Price { get; set; }
	public DateTime? Date { get; set; }
}
