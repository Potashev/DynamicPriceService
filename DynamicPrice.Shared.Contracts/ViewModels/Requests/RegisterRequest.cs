using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.ViewModels.Requests;

public class RegisterRequest
{
	[Required]
	public string Username { get; init; } = null!;

	[Required, EmailAddress]
	public string Email { get; init; } = null!;

	[Required, MinLength(6)]
	public string Password { get; init; } = null!;

	[Required]
	public string Role { get; init; } = null!;
}
