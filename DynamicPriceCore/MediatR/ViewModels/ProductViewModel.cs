namespace DynamicPriceCore.MediatR.ViewModels;

public class ProductViewModel
{
	public int ProductId { get; set; }
	public string Title { get; set; }
	public double Price { get; set; }
	public double MinimumPrice { get; set; }
	public int? Quantity { get; set; }
	public string? Description { get; set; }
}
