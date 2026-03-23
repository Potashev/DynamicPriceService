using DynamicPrice.Core.Models;
using Microsoft.Build.Evaluation;

namespace DynamicPrice.Core.Services;

// todo: make from applicationUser?
public class VirtualCustomer
{
	public string Id { get; set; }
	public int ThresholdPercent { get; set; }

	public List<Product> MonitorProducts(Product[] products) 
	{
		var random = new Random();

		//todo: check
		var interestedProducts = products
			.OrderBy(x => random.Next())
			.Take(3)	//todo: fixed or optimized
			.ToList();

		var productsToBuy = new List<Product>();

		foreach (var product in interestedProducts)
		{
			// получаем среднюю цену продукта
			var averagePrice = product.PriceDynamics.Average(pd => pd.Price); //todo: check

			// получаем допустимую цену по которой готовы взять
			var maxPricetoBuy = averagePrice * 0.01m * ThresholdPercent + averagePrice;

			// сравниваем с текущей и добавляем к покупке
			if (product.Price <= maxPricetoBuy)
				productsToBuy.Add(product);

		}

		// результатом работы будет список продуктов, которые будем в заказ оформлять
		return productsToBuy;
	}
}
