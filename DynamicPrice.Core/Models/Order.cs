using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Core.Models;

[Index(nameof(Number), IsUnique = true)]
public class Order
{
	public int OrderId { get; set; }    //todo: make guid

	[MaxLength(20)]
	public string Number { get; set; }
	public string CustomerId { get; set; }
	public Company Company { get; set; }
	public ICollection<OrderItem> OrderItems { get; set; }
	public OrderStatus Status { get; set; }
	public DateTime? OrderDate { get; set; }
	public int ReceiveKey { get; set; }
}

//todo: moved to shared.contracts?
public enum OrderStatus
{
	Confirmed,
	Ready,
	Completed,
	Canceled
}
