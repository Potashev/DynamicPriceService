namespace DynamicPriceCore.Models;

public class Order
{
	public int OrderId { get; set; }
	public Customer Customer { get; set; }
	public Company Company { get; set; }
	public ICollection<OrderProduct> OrderProducts { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime? OrderDate { get; set; }
}

public enum OrderStatus
{
	Cart,
	Confirmed,
	Completed
}
