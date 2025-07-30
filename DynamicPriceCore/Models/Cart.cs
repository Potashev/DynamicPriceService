namespace DynamicPriceCore.Models;

public class Cart
{
	public int CartId { get; set; }
	public ApplicationUser Customer { get; set; }
	public Company Company { get; set; }
	public ICollection<CartItem> CartItems { get; set; }
}
