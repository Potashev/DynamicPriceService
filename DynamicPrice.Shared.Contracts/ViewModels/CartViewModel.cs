namespace DynamicPrice.Shared.Contracts.ViewModels;

public class CartViewModel
{
	public Guid Id { get; init; }
	public Guid CompanyId { get; init; }
	public CompanyViewModel Company { get; init; } = null!;
	public ICollection<CartItemViewModel> CartItems { get; init; } = [];
}
