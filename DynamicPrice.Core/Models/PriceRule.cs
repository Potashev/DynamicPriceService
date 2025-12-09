using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Правило изменения цены для компании.
/// Содержит параметры для повышения и снижения цен и время простоя для снижения.
/// </summary>
public class PriceRule
{
	/// <summary>
	/// Идентификатор правила.
	/// </summary>
	public int PriceRuleId { get; set; }

	/// <summary>
	/// Компания, к которой применяется правило.
	/// </summary>
	public Company Company { get; set; }

	/// <summary>
	/// Внешний ключ на компанию.
	/// </summary>
	public int? CompanyId { get; set; }     //todo: make required

	/// <summary>
	/// Повышение цены продукта (в процентах).
	/// Значение 10 означает повышение на 10%.
	/// </summary>
	public double Increase { get; set; }

	/// <summary>
	/// Снижение цены продукта (в процентах).
	/// Значение 10 означает снижение на 10%.
	/// </summary>
	public double Reduction { get; set; }

	/// <summary>
	/// Допустимое время "простоя" продукта. Если продукт не продавался дольше этого времени — применяется снижение цены.
	/// </summary>
	[NotMapped]
	public TimeSpan NoSellTime
	{
		get => TimeSpan.FromSeconds(NoSellSeconds);
		set => NoSellSeconds = (int)value.TotalSeconds;
	}

	/// <summary>
	/// Время простоя в секундах (сериализуемое/сохраняемое поле).
	/// </summary>
	public int NoSellSeconds { get; set; }

	//todo: think about about monitor waiting config - time before next monitoring as active company
}
