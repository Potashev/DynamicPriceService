namespace DynamicPrice.Shared.Contracts.ViewModels;

public class OrderViewModel
{
	public int OrderId { get; init; }
	public string Number { get; init; } = null!;
	public string CustomerId { get; init; } = null!;
	public string CustomerName { get; set; } = null!;
	public CompanyViewModel Company { get; init; } = null!;	//try to use companyId for companyOrders
	public ICollection<OrderItemViewModel> OrderItems { get; init; } = [];	//try not to get for companyOrders
	public OrderStatus Status { get; init; }
	public DateTime OrderDate { get; init; }
	public int? ReceiveKey { get; init; } = null;
	public decimal OrderTotal =>
		OrderItems?.Sum(i => i.ProductPrice * i.Quantity) ?? 0m;
}

public enum OrderStatus
{
	Confirmed,
	Ready,
	Completed,
	Canceled
}
