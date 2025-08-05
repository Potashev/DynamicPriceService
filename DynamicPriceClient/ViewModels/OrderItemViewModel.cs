namespace DynamicPriceClient.ViewModels;

public class OrderItemViewModel
{
	public int Id { get; set; }
	public ProductViewModel Product { get; set; }
	public double? ProductPrice { get; set; }
	public int Quantity { get; set; }
}
