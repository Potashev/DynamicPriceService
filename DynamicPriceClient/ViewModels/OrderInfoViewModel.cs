using DynamicPriceClient.ViewModels;

namespace DynamicPriceClient.ViewModels;

public class OrderInfoViewModel
{
	public int OrderId { get; set; }
	public CompanyViewModel Company { get; set; }
	public ICollection<OrderProductViewModel> OrderProducts { get; set; }
	public OrderStatus Status { get; set; }
	public DateTime? OrderDate { get; set; }

	public double? OrderAmount { get; set; }
}

public enum OrderStatus
{
	Cart,       //todo: obsolete - remove
	Confirmed,
	Completed
}