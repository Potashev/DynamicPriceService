namespace DynamicPrice.Shared.Contracts.ViewModels.Responses;

/// <summary>
/// Dto продукта для покупателя.
/// </summary>
public class ProductInfoViewModel
{
	public Guid ProductId { get; init; }
	public required string Title { get; init; }
	public decimal Price { get; init; }
	public int? Quantity { get; init; }
	public string? Description { get; init; }
	public ICollection<PriceDynamicViewModel> PriceDynamics { get; init; } = [];
}
