using DynamicPriceCore.Models;

namespace DynamicPriceCore.MediatR.ViewModels;

public class OrderViewModel
{
	public int OrderId { get; set; }
	public ICollection<OrderProductViewModel> OrderProducts { get; set; }
	public OrderStatus Status { get; set; }
	public DateTime? OrderDate { get; set; }
	public double? OrderAmount { get; set; }    // additional prop
}
