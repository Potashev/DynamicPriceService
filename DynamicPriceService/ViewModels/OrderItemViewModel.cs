namespace DynamicPriceService.ViewModels;

public class OrderItemViewModel
{
	public int Id { get; set; }
	public ProductViewModel Product { get; set; }
	public double? Price { get; set; }
	public int Quantity { get; set; }
}
