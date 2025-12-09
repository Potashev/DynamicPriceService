namespace DynamicPrice.Core.Models;

/// <summary>
/// Сущность, используемая для мониторинга активности компании (временные метки последней проверки и момента старта наблюдения).
/// </summary>
public class ActiveCompany
{
	/// <summary>
	/// Идентификатор компании.
	/// </summary>
	public int CompanyId { get; set; }

	/// <summary>
	/// Временная метка начала наблюдения (UTC).
	/// </summary>
	public DateTime StartedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Временная метка последней проверки активности.
	/// </summary>
	public DateTime? LastMonitoring { get; set; }

	/// <summary>
	/// Навигационное свойство на сущность компании (опционально).
	/// </summary>
	public Company? Company { get; set; }
}