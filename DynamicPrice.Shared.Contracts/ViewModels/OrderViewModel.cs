namespace DynamicPrice.Shared.Contracts.ViewModels;

public class OrderViewModel
{
	public int OrderId { get; init; }
	public string Number { get; init; } = null!;
	public string CustomerId { get; init; } = null!;
	public string CustomerName { get; set; } = null!;
	public CompanyViewModel Company { get; init; } = null!;
	public ICollection<OrderItemViewModel> OrderItems { get; init; } = [];
	public OrderStatus Status { get; init; }
	public DateTime OrderDate { get; init; }
	public int ReceiveKey { get; init; }  //todo: check after removed nullable

	//todo: calculate on server side
	public decimal OrderTotal =>
		OrderItems?.Sum(i => i.ProductPrice * i.Quantity) ?? 0m;
}
