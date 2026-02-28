using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Продукт компании.
/// </summary>
public class Product
{
	/// <summary>
	/// Идентификатор продукта.
	/// </summary>
	public int ProductId { get; set; }  //todo: make Guid

	/// <summary>
	/// Идентификатор компании.
	/// </summary>
	public int CompanyId { get; set; }

	/// <summary>
	/// Навигационнное свойство компании, к которой принадлежит продукт.
	/// </summary>
	public Company Company { get; set; } = null!;

	/// <summary>
	/// Название продукта.
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// Текущая цена продукта.
	/// </summary>
	[Precision(18, 2)]
	public decimal Price { get; set; }

	/// <summary>
	/// Минимальная допустимая цена для продукта.
	/// </summary>
	public decimal MinimumPrice { get; set; }

	/// <summary>
	/// Доступное количество на складе (nullable — может быть не задано для неинвентаризируемых товаров).
	/// </summary>
	public int? Quantity { get; set; }

	/// <summary>
	/// Описание товара (опционально).
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Время последней продажи товара. Используется для определения "простоя" продукта.
	/// </summary>
	public DateTime? LastSellTime { get; set; }

	/// <summary>
	/// История изменений цены для данного товара.
	/// </summary>
	public ICollection<PriceDynamic> PriceDynamics { get; set; } = [];
}
