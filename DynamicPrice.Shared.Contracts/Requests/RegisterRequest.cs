using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.Requests;

public class RegisterRequest
{
	[Required]
	public string Username { get; init; } = null!;

	[Required, EmailAddress]
	public string Email { get; init; } = null!;

	[Required, MinLength(6)]
	public string Password { get; init; } = null!;
}
