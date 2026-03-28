using DynamicPrice.Core.Models;

namespace DynamicPrice.Core.Services;

public class VirtualCustomer
{
	private const int PRODUCTS_NUMBER_FOR_MONITORING = 2;
	private const int MAX_PRODUCTS_QUANTITY_FOR_BUYING = 4;
	private const int MAX_NEXT_MONITOR_MILLISECONDS = 5000;
	private const string ID_PREFIX = "virt-cust";

	private static readonly Random _rnd = new();

	public string CustomerId { get; }
	public int ThresholdPercent { get; }

	public List<CartItem> MonitorProducts(Product[] products) 
	{
		var interestedProducts = SelectRandomProducts(products);

		var productsToBuy = new List<CartItem>();

		foreach (var product in interestedProducts)
		{
			var averagePrice = product.PriceDynamics.Any()
				? product.PriceDynamics.Average(pd => pd.Price)
				: product.Price;

			var maxPriceToBuy = GetMaximumBuyPrice(averagePrice);

			if (product.Price <= maxPriceToBuy)
				productsToBuy.Add(new CartItem 
				{ 
					Product = product, 
					Quantity = SelectProductQuantity()
				});

		}

		return productsToBuy;
	}

	private Product[] SelectRandomProducts(Product[] products)
		=> products
		.OrderBy(x => _rnd.Next())
		.Take(PRODUCTS_NUMBER_FOR_MONITORING)
		.ToArray();

	private decimal GetMaximumBuyPrice(decimal averagePrice)
		=> averagePrice * (1 + ThresholdPercent / 100m);

	private int SelectProductQuantity()
		=> _rnd.Next(MAX_PRODUCTS_QUANTITY_FOR_BUYING) + 1;

	public static string IdPrefix
		=> ID_PREFIX;

	public VirtualCustomer(int thresholdPercent)
	{
		ThresholdPercent = thresholdPercent;
		CustomerId = $"{IdPrefix}-{Guid.NewGuid()}";
	}

	public static bool IsVirtualCustomer(string customerId)
		=> customerId.StartsWith(IdPrefix);

	public static TimeSpan WaitNextMonitor()
		=> TimeSpan.FromMilliseconds(_rnd.Next(MAX_NEXT_MONITOR_MILLISECONDS));
}
