namespace DynamicPrice.Shared.Contracts.Requests;

public class RegisterUserRequest
{
	public string Username { get; init; } = null!;
	public string Email { get; init; } = null!;
	public string Password { get; init; } = null!;

	public string Role { get; init; } = null!;
	public decimal? Balance { get; init; }
	public int? CompanyId { get; init; }

	public RegisterUserRequest(RegisterRequest request)
	{
		Username = request.Username;
		Email = request.Email;
		Password = request.Password;
	}
}
