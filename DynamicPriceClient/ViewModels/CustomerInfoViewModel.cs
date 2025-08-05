namespace DynamicPriceClient.ViewModels;

public class CustomerInfoViewModel
{
	public string Name { get; set; }
	public decimal Balance { get; set; }
	public ICollection<OrderInfoViewModel> Orders { get; set; }
}
