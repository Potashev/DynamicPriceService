using DynamicPriceClient.ViewModels;

namespace DynamicPrice.Customer.ViewModels;

public class CustomerInfoViewModel
{
	public string Name { get; set; }
	public string Email { get; set; }
	public decimal Balance { get; set; }
	public ICollection<OrderInfoViewModel> Orders { get; set; }
}
