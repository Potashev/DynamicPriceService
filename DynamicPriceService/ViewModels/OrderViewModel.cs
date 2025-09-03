namespace DynamicPriceService.ViewModels;

public class OrderViewModel
{
	public int OrderId { get; set; }
	public string CustomerId { get; set; }
	public string CustomerName { get; set; }
	public ICollection<OrderItemViewModel> OrderItems { get; set; }
	public OrderStatus Status { get; set; }
	public DateTime? OrderDate { get; set; }

	public double? OrderAmount { get; set; }
}

public enum OrderStatus
{
	Confirmed,
	Completed
}