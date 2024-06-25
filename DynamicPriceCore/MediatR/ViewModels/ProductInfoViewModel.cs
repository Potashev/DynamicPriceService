namespace DynamicPriceCore.MediatR.ViewModels;

/// <summary>
/// Dto продукта для покупателя.
/// </summary>
public class ProductInfoViewModel
{
	public int ProductId { get; set; }
	public string Title { get; set; }
	public double Price { get; set; }
	public int? Quantity { get; set; }
	public string? Description { get; set; }
}
