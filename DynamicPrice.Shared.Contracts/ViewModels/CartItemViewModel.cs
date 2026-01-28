namespace DynamicPrice.Shared.Contracts.ViewModels;

public class CartItemViewModel
{
	public int Id { get; init; }
	public int CartId { get; init; }
	public int ProductId { get; init; }
	public ProductViewModel Product { get; init; } = null!;
	public int Quantity { get; init; }
}
