namespace DynamicPrice.Shared.Contracts.ViewModels.Entities;

public class OrderItemViewModel
{
	public int Id { get; set; }
	public ProductViewModel Product { get; set; }
	public decimal? ProductPrice { get; set; }
	public int Quantity { get; set; }
}
