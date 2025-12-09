namespace DynamicPrice.Core.Models;

/// <summary>
/// Компания-продавец.
/// </summary>
public class Company
{
	/// <summary>
	/// Идентификатор компании.
	/// </summary>
	public int CompanyId { get; set; }

	/// <summary>
	/// Название компании.
	/// </summary>
	public string Title { get; set; }
}
