namespace DynamicPrice.Shared.Contracts.ViewModels;

public class OrdersStatistics
{
	public int OrdersQuantity { get; set; }
	public decimal TotalAmount { get; set; }
	public decimal AverageOrderTotal { get; set; }
}
