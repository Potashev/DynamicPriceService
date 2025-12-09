namespace DynamicPrice.Core.Models;

/// <summary>
/// История изменения цены товара (снимок цены в определённый момент времени).
/// </summary>
public class PriceDynamic
{
	/// <summary>
	/// Идентификатор записи истории цены.
	/// </summary>
	public int Id { get; set; }     //todo: make guid

	/// <summary>
	/// Идентификатор товара, к которому относится запись.
	/// </summary>
	public int ProductId { get; set; }

	/// <summary>
	/// Цена товара в момент записи.
	/// </summary>
	public decimal Price { get; set; }

	/// <summary>
	/// Время записи цены (UTC).
	/// </summary>
	public DateTime? Date { get; set; }
}
