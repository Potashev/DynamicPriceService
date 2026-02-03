namespace DynamicPrice.Shared.Contracts.ViewModels;

public class PriceDynamicViewModel
{
	public int Id { get; init; }
	public int ProductId { get; init; }
	public decimal Price { get; init; }
	public DateTime Date { get; init; }
}
