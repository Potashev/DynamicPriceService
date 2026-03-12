namespace DynamicPrice.Shared.Contracts.Requests;

//todo: remove from request?
public class RegisterUserRequest
{
	public string Username { get; init; } = null!;
	public string Email { get; init; } = null!;
	public string Password { get; init; } = null!;
	public string Role { get; set; } = null!;

	public decimal Balance { get; set; }

	public int? CompanyId { get; set; }

	public RegisterUserRequest(RegisterRequest request)
	{
		Username = request.Username;
		Email = request.Email;
		Password = request.Password;
	}
}
