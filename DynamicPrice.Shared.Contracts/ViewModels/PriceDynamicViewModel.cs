namespace DynamicPrice.Shared.Contracts.ViewModels;

public class PriceDynamicViewModel
{
	public Guid Id { get; init; }
	public Guid ProductId { get; init; }
	public decimal Price { get; init; }
	public DateTime Date { get; init; }
}
