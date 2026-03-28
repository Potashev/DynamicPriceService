using DynamicPrice.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Продукт компании.
/// </summary>
public class Product
{
	/// <summary>
	/// Идентификатор продукта.
	/// </summary>
	public int ProductId { get; set; }

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

	public void IncreaseQuantity(int amount)
	{
		if (Quantity is null)
			return;

		Quantity += amount;
	}

	public void ReduceQuantity(int amount)
	{
		if (Quantity is null)
			return;

		if (Quantity < amount)
			throw new BusinessException("Not enough products");

		Quantity -= amount;
	}

	public void UpdateLastSellTime(DateTime time)
		=> LastSellTime = time;

	public static Expression<Func<Product, bool>> CanBeReducedExpr =>
		p => p.Quantity == null || p.Quantity > 0;
}
