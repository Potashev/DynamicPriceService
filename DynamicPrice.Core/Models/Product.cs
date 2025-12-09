using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Продукт (товар) компании — сущность предметной области, используемая в каталоге и заказах.
/// </summary>
public class Product    //todo: rename to Item?
{
	/// <summary>
	/// Идентификатор товара.
	/// </summary>
	public int ProductId { get; set; }  //todo: make Guid

	/// <summary>
	/// Компания-продавец, которой принадлежит товар.
	/// </summary>
	public Company Company { get; set; }

	/// <summary>
	/// Внешний ключ на компанию (nullable для возможности загрузки/миграций).
	/// </summary>
	public int? CompanyId { get; set; }

	/// <summary>
	/// Название товара.
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// Текущая цена товара. Тип и precision заданы атрибутом <see cref="PrecisionAttribute"/>.
	/// </summary>
	[Precision(18, 2)]
	public decimal Price { get; set; }

	/// <summary>
	/// Минимальная допустимая цена для товара. Во избежание снижения ниже этого значения.
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
	/// Время последней продажи товара (UTC). Используется для принятия решений о динамике цены.
	/// </summary>
	public DateTime? LastSellTime { get; set; }

	/// <summary>
	/// История изменений цены для данного товара.
	/// </summary>
	public ICollection<PriceDynamic> PriceDynamics { get; set; }
}
