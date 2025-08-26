using DynamicPriceClient.ViewModels;
using Refit;

namespace DynamicPriceClient.ApiClients;

public interface ICoreApiClient
{
	[Get("/api/companies?status=active")]
	Task<IEnumerable<CompanyViewModel>> GetCompanies();

	[Get("/api/companies/{companyId}/products")]
	Task<CompanyProductsInfo> GetCompanyProducts(int companyId);

	[Get("/api/customers/me")]
	Task<CustomerInfoViewModel> GetCustomer();

	[Put("/api/customers/me/balance")]
	Task TopUpBalance(BalanceViewModel balanceVm);

	[Post("/api/auth/register")]
	Task RegisterCustomer(RegisterViewModel registerVm);

	[Post("/api/auth/login")]
	Task<TokenResponse> LoginCustomer(LoginViewModel loginVm);

	[Get("/api/carts/{companyId}")]
	Task<CartViewModel> GetCartDetails(string companyId);

	[Post("/api/carts/items")]
	Task<int> AddProduct([Body] int productId);

	[Delete("/api/carts/items/{productId}")]
	Task<int> DeleteProduct(int productId);

	[Post("/api/orders")]
	Task<int> ConfirmOrder([Body] int cartId);
}
