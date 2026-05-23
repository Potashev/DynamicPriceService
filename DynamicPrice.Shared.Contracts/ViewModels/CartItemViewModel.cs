namespace DynamicPrice.Shared.Contracts.ViewModels;

public class CartItemViewModel
{
	public Guid Id { get; init; }
	public Guid CartId { get; init; }
	public Guid ProductId { get; init; }
	public ProductViewModel Product { get; init; } = null!;
	public int Quantity { get; init; }
}
