using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.ViewModels.Requests;

public class LoginRequest
{
	[Required]
	public string Username { get; init; } = null!;

	[Required]
	public string Password { get; init; } = null!;
}
