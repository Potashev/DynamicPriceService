namespace DynamicPrice.Shared.Contracts.ViewModels;

public class ProductViewModel
{
	//[Required]   // todo: check
	public int ProductId { get; init; }
	public string Title { get; init; } = null!;
	public decimal Price { get; init; }
	public decimal MinimumPrice { get; init; }
	public int? Quantity { get; init; }
	public string? Description { get; init; }
	public ICollection<PriceDynamicViewModel> PriceDynamics { get; set; } = [];
}
