namespace DynamicPrice.Core.Models;

public class Cart
{
	public int CartId { get; set; } //todo: make guid?
	public string CustomerId { get; set; }
	public Company Company { get; set; }
	public ICollection<CartItem> CartItems { get; set; }
}
