using Microsoft.AspNetCore.Identity;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Пользователь приложения, расширяющий стандартную IdentityUser для хранения баланса и связи с компанией.
/// </summary>
public class ApplicationUser : IdentityUser
{
	/// <summary>
	/// Баланс пользователя в денежной единице, используемой в системе.
	/// </summary>
	public decimal Balance { get; set; }

	/// <summary>
	/// Идентификатор компании, с которой связан пользователь (например, для менеджеров/сотрудников).
	/// </summary>
	public int? CompanyId { get; set; }
}
