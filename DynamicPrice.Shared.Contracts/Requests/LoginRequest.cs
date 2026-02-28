using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.Requests;

public class LoginRequest
{
	[Required]
	public string Username { get; init; } = null!;

	//todo: pass and contains password hash?
	[Required]
	public string Password { get; init; } = null!;
}
