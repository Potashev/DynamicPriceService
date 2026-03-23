using DynamicPrice.Core.Models;
using Microsoft.Build.Evaluation;

namespace DynamicPrice.Core.Services;

// todo: make from applicationUser?
public class VirtualCustomer
{
	public string Id { get; set; }
	public int ThresholdPercent { get; set; }

	public List<CartItem> MonitorProducts(Product[] products) 
	{
		var rnd = new Random();

		var interestedProducts = products
			.OrderBy(x => rnd.Next())
			.Take(3)	//todo: fixed or optimized
			.ToList();

		var productsToBuy = new List<CartItem>();

		foreach (var product in interestedProducts)
		{
			// получаем среднюю цену продукта
			var averagePrice = product.PriceDynamics.Average(pd => pd.Price); //todo: check

			// получаем допустимую цену по которой готовы взять
			var maxPricetoBuy = averagePrice * 0.01m * ThresholdPercent + averagePrice;

			// сравниваем с текущей и добавляем к покупке
			if (product.Price <= maxPricetoBuy)
				productsToBuy.Add(new CartItem { Product = product, Quantity = rnd.Next(3) + 1 });	//todo: quantity can depend on product.price/maxPriceTobuy value

		}

		// результатом работы будет список продуктов, которые будем в заказ оформлять
		return productsToBuy;
	}
}
