using DynamicPrice.Client.Common.ViewModels;

namespace DynamicPrice.Customer.ViewModels;

public class CartViewModel
{
	public int CartId { get; set; }
	public CompanyViewModel Company { get; set; }
	public ICollection<CartItemViewModel> CartItems { get; set; }
}
