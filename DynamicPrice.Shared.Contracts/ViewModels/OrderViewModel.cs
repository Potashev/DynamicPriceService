namespace DynamicPrice.Shared.Contracts.ViewModels;

public class OrderViewModel
{
	public int OrderId { get; set; }
	public string Number { get; set; }
	public string CustomerId { get; set; }
	public string CustomerName { get; set; }
	public ICollection<OrderItemViewModel> OrderItems { get; set; }
	public OrderStatus Status { get; set; }
	public DateTime? OrderDate { get; set; }
	//public decimal? OrderTotal { get; set; }
	public decimal? OrderTotal =>
		OrderItems?.Sum(i => i.ProductPrice * i.Quantity) ?? 0m;
}
