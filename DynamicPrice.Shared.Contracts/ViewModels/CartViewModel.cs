namespace DynamicPrice.Shared.Contracts.ViewModels;

public class CartViewModel
{
	public int CartId { get; init; }
	public int CompanyId { get; init; }
	public CompanyViewModel Company { get; init; } = null!;
	public ICollection<CartItemViewModel> CartItems { get; init; } = [];
}
