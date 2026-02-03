namespace DynamicPrice.Shared.Contracts.ViewModels;

public class OrderItemViewModel
{
	public int Id { get; init; }
	public ProductViewModel Product { get; init; } = null!;
	public decimal ProductPrice { get; init; }  //todo: check after removed nullable
	public int Quantity { get; init; }
}
