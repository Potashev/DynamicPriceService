namespace DynamicPrice.Core.Models;

/// <summary>
/// Сущность, используемая для проверки активности компании - для активных компаний происходит мониториг продуктов с простоем.
/// См. также <see cref="Models.Company"/> и <see cref="Product"/>.
/// </summary>
public class ActiveCompany
{
	/// <summary>
	/// Идентификатор компании.
	/// </summary>
	public int CompanyId { get; set; }

	/// <summary>
	/// Временная метка начала мониторинга.
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