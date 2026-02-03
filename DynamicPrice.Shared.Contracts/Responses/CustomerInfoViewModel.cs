namespace DynamicPrice.Shared.Contracts.ViewModels.Responses;

public class CustomerInfoViewModel
{
	public required string Name { get; init; }
	public required string Email { get; init; }
	public decimal Balance { get; init; }
	public ICollection<OrderViewModel> Orders { get; init; } = [];
}
