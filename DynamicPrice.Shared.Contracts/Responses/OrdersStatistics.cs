namespace DynamicPrice.Shared.Contracts.ViewModels.Responses;

public class OrdersStatistics
{
	public int OrdersQuantity { get; init; }
	public decimal TotalAmount { get; init; }
	public decimal AverageOrderTotal { get; init; }
}
