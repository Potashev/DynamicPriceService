using DynamicPrice.Core.Models;

namespace DynamicPrice.Core.Services;

public class VirtualCustomer
{
	const int PRODUCTS_NUMBER_FOR_MONITORING = 3;
	const int MAX_PRODUCTS_NUMBER_FOR_BUYING = 3;
	const int MAX_NEXT_MONITOR_MILLISECONDS = 7000;
	const string ID_PREFIX = "virt-cust";

	//private static List<VirtualCustomer> _virtualCustomers = new ();
	private static readonly Random _rnd = new();

	public string CustomerId { get; }
	public int ThresholdPercent { get; }

	public List<CartItem> MonitorProducts(Product[] products) 
	{
		var interestedProducts = SelectRandomProducts(products);

		var productsToBuy = new List<CartItem>();

		foreach (var product in interestedProducts)
		{
			// получаем среднюю цену продукта
			//var averagePrice = product.PriceDynamics.Average(pd => pd.Price);
			var averagePrice = product.PriceDynamics.Any()	//todo: check
				? product.PriceDynamics.Average(pd => pd.Price)
				: product.Price;

			// получаем допустимую цену по которой готовы взять
			var maxPriceToBuyOLD = averagePrice * 0.01m * ThresholdPercent + averagePrice;

			var maxPriceToBuy = averagePrice * (1 + ThresholdPercent / 100m);	//todo: check

			// сравниваем с текущей и добавляем к покупке
			if (product.Price <= maxPriceToBuy)
				productsToBuy.Add(new CartItem 
				{ 
					Product = product, 
					Quantity = _rnd.Next(MAX_PRODUCTS_NUMBER_FOR_BUYING) + 1 
				});	//todo: quantity can depend on product.price/maxPriceTobuy value

		}

		// результатом работы будет список продуктов, которые будем в заказ оформлять
		return productsToBuy;
	}

	private Product[] SelectRandomProducts(Product[] products)
		=> products
		.OrderBy(x => _rnd.Next())
		.Take(PRODUCTS_NUMBER_FOR_MONITORING)
		.ToArray();

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
