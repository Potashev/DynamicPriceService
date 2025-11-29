using DynamicPrice.Client.Common;
using DynamicPriceClient.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Customer.ViewModels;

public class OrderInfoViewModel
{
	public int OrderId { get; set; }

	[Display(Name = "Order number")]
	public string Number { get; set; }
	public CompanyViewModel Company { get; set; }
	public ICollection<OrderItemViewModel> OrderItems { get; set; }
	public OrderStatus Status { get; set; }

	[Display(Name = "Order date")]
	public DateTime? OrderDate { get; set; }

	[Display(Name = "Order total")]
	public decimal? OrderTotal { get; set; }

	[Display(Name = "Receive key")]
	public int? ReceiveKey { get; set; }
}