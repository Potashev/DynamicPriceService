namespace DynamicPrice.Core.ViewModels;

public class CartViewModel
{
	public int CartId { get; set; }
	public CompanyViewModel Company { get; set; }
	public ICollection<CartItemViewModel> CartItems { get; set; }
}
