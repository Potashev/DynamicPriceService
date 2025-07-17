using DynamicPriceCore.Models;

namespace DynamicPriceCore.MediatR.ViewModels;

public class OrderInfoViewModel
{
	public int OrderId { get; set; }
	public Company Company { get; set; }
	public ICollection<OrderItemViewModel> OrderItems { get; set; }	//todo: check after replacing from OrderProducts
	public OrderStatus Status { get; set; }
	public DateTime? OrderDate { get; set; }

	public double? OrderAmount { get; set; }
}

//public enum OrderStatus
//{
//	Cart,
//	Confirmed,
//	Completed
//}
