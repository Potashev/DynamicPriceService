using DynamicPriceClient.ViewModels;

namespace DynamicPriceClient.ViewModels;

public class OrderInfoViewModel
{
	public int OrderId { get; set; }
	public CompanyViewModel Company { get; set; }
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