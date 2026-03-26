using DynamicPrice.Core.Models;

namespace DynamicPrice.Core.Services;

public class VirtualCustomer
{
	const int PRODUCTS_NUMBER_FOR_MONITORING = 3;
	const int MAX_PRODUCTS_NUMBER_FOR_BUYING = 3;
	const int MAX_NEXT_MONITOR_MILLISECONDS = 7000;
	const string CUSTOMER_ID = "virt-cust";

	private static List<VirtualCustomer> _virtualCustomers;

	public string CustomerId { get; }
	public int ThresholdPercent { get; }

	public List<CartItem> MonitorProducts(Product[] products) 
	{
		var rnd = new Random();

		var interestedProducts = products
			.OrderBy(x => rnd.Next())	//todo: check
			.Take(PRODUCTS_NUMBER_FOR_MONITORING)
			.ToList();

		var productsToBuy = new List<CartItem>();

		foreach (var product in interestedProducts)
		{
			// получаем среднюю цену продукта
			var averagePrice = product.PriceDynamics.Average(pd => pd.Price);

			// получаем допустимую цену по которой готовы взять
			var maxPricetoBuy = averagePrice * 0.01m * ThresholdPercent + averagePrice;

			// сравниваем с текущей и добавляем к покупке
			if (product.Price <= maxPricetoBuy)
				productsToBuy.Add(new CartItem 
				{ 
					Product = product, 
					Quantity = rnd.Next(MAX_PRODUCTS_NUMBER_FOR_BUYING) + 1 
				});	//todo: quantity can depend on product.price/maxPriceTobuy value

		}

		// результатом работы будет список продуктов, которые будем в заказ оформлять
		return productsToBuy;
	}

	public static VirtualCustomer GetCustomer()
		=> _virtualCustomers[new Random().Next(_virtualCustomers.Count)];

	public static string Id
		=> CUSTOMER_ID;

	private VirtualCustomer(int thresholdPercent)
	{
		ThresholdPercent = thresholdPercent;
		CustomerId = Id;
	}

	public static void CreateCustomersPool()	//todo: add int customersCount with each VirtualCustomer(rnd.Next())...
		=> _virtualCustomers =
		[
			new VirtualCustomer(5),
			new VirtualCustomer(3),
			new VirtualCustomer(1)
		];

	public static TimeSpan WaitNextMonitor()
		=> TimeSpan.FromMilliseconds(new Random().Next(MAX_NEXT_MONITOR_MILLISECONDS));
}
