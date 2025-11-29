using DynamicPrice.Client.Common;
using DynamicPrice.Client.Common.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Manager.ViewModels;

public class OrderViewModel
{
	public int OrderId { get; set; }

	[Display(Name = "Order number")]
	public string Number { get; set; }
	public string CustomerId { get; set; }

	[Display(Name = "Customer")]
	public string CustomerName { get; set; }
	public ICollection<OrderItemViewModel> OrderItems { get; set; }
	public OrderStatus Status { get; set; }

	[Display(Name = "Order date")]
	public DateTime? OrderDate { get; set; }

	[Display(Name = "Order total")]
	public decimal? OrderTotal { get; set; }
}