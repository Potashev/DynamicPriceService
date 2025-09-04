using DynamicPriceClient.ViewModels;
using Refit;

namespace DynamicPriceClient.ApiClients;

public interface ICoreApiClient
{
	[Get("/api/companies?status=active")]
	Task<IEnumerable<CompanyViewModel>> GetCompanies();

	[Get("/api/companies/{companyId}/products")]
	Task<CompanyProductsInfo> GetCompanyProducts(int companyId);

	[Get("/api/customer/me")]
	Task<CustomerInfoViewModel> GetCustomer();

	[Put("/api/customer/me/balance")]
	Task TopUpBalance(BalanceViewModel balanceVm);

	[Post("/api/auth/register")]
	Task RegisterCustomer(RegisterViewModel registerVm);

	[Post("/api/auth/login")]
	Task<TokenResponse> LoginCustomer(LoginViewModel loginVm);

	[Get("/api/cart?company-id={companyId}")]
	Task<CartViewModel> GetCartDetails(string companyId);

	[Post("/api/cart/items")]
	Task<int> AddProduct([Body] int productId);

	[Delete("/api/cart/items/{productId}")]
	Task<int> DeleteProduct(int productId);

	[Post("/api/customer/order/confirm")]
	Task<int> ConfirmOrder([Body] int cartId);

	[Get("/api/customer/order/{orderId}")]
	Task<OrderInfoViewModel> OrderDetails(int orderId);

	//[Post("/api/orders")]
	//Task<int> CancelOrder([Body] int cartId);
}
