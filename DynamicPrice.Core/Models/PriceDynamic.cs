using DynamicPrice.Core.Services;

namespace DynamicPrice.Core.Models;

/// <summary>
/// История изменения цены товара (снимок цены в определённый момент времени).
/// Динамика изменения цены продукта, в зависимости от спроса: совершили заказа с продуктом - цена повышается, продукт с простоем - цена снижается.
/// См. также <see cref="PriceIncreaseEvent"/> и <see cref="PriceReduceEvent"/>.
/// </summary>
public class PriceDynamic
{
	/// <summary>
	/// Идентификатор.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Идентификатор продукта.
	/// </summary>
	public Guid ProductId { get; set; }

	/// <summary>
	/// Цена продукта на момент записи.
	/// </summary>
	public decimal Price { get; set; }

	/// <summary>
	/// Время записи цены.
	/// </summary>
	public DateTime Date { get; set; }
}
