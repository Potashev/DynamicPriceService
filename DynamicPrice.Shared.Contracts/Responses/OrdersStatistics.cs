namespace DynamicPrice.Shared.Contracts.ViewModels.Responses;

public class OrdersStatistics
{
	public int OrdersQuantity { get; set; }
	public decimal TotalAmount { get; set; }
	public decimal AverageOrderTotal { get; set; }
}
