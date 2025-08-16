namespace DynamicPriceCore.Models;

public class Order
{
	public int OrderId { get; set; }
	public string CustomerId { get; set; }
	public Company Company { get; set; }
	public ICollection<OrderItem> OrderItems { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime? OrderDate { get; set; }
	public int ReceiveKey { get; set; }
}

public enum OrderStatus
{
	Confirmed,
	Completed
}
