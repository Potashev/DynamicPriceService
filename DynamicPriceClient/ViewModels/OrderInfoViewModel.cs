using DynamicPriceCore.Models;

namespace DynamicPriceClient.ViewModels;

public class OrderInfoViewModel
{
	public int OrderId { get; set; }
	public Company Company { get; set; }
	public ICollection<OrderProductViewModel> OrderProducts { get; set; }
	public OrderStatus Status { get; set; }
	public DateTime? OrderDate { get; set; }

	public double? OrderAmount { get; set; }
}