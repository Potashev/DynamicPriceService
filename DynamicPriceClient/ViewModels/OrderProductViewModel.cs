namespace DynamicPriceClient.ViewModels;

public class OrderProductViewModel
{
	public int Id { get; set; }
	public ProductViewModel Product { get; set; }
	public double? Price { get; set; }
	public int Quantity { get; set; }
}
