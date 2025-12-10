using Microsoft.AspNetCore.Identity;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Пользователь приложения, расширяющий стандартную IdentityUser для хранения баланса и связи с компанией.
/// </summary>
public class ApplicationUser : IdentityUser
{
	/// <summary>
	/// Баланс пользователя в денежной единице, используемой в системе (кастомер).
	/// </summary>
	public decimal Balance { get; set; }

	/// <summary>
	/// Идентификатор компании, с которой связан пользователь (менеджер).
	/// См. также <see cref="DynamicPrice.Core.Models.Company"/>.
	/// </summary>
	public int? CompanyId { get; set; }
}
