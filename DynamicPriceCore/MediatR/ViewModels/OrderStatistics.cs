namespace DynamicPriceCore.MediatR.ViewModels;

public class OrderStatistics
{
    public int OrdersQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageOrderAmount { get; set; }
}
