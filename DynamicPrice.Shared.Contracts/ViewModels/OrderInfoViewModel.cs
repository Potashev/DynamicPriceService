namespace DynamicPrice.Shared.Contracts.ViewModels;

//todo: rename?
public class OrderInfoViewModel
{
	public int OrderId { get; set; }
	public string Number { get; set; }
	public CompanyViewModel Company { get; set; }   //todo: check
	public ICollection<OrderItemViewModel> OrderItems { get; set; }
	public OrderStatus Status { get; set; }
	public DateTime? OrderDate { get; set; }
	public decimal? OrderTotal { get; set; }
	public int? ReceiveKey { get; set; }
}
