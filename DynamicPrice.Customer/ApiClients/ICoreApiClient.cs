using DynamicPrice.Shared.Contracts.Requests;
using DynamicPrice.Shared.Contracts.Responses;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using Refit;

namespace DynamicPrice.Customer.ApiClients;

public interface ICoreApiClient
{
	[Get("/api/companies?status=active")]
	Task<IEnumerable<CompanyViewModel>> GetCompanies();

	[Get("/api/companies/{companyId}/products")]
	Task<CompanyProductsInfo> GetCompanyProducts(int companyId);

	//[Post("/api/auth/register")]
	//Task RegisterCustomer(RegisterRequest registerVm);

	[Post("/api/auth/login")]
	Task<TokenResponse> LoginCustomer(LoginRequest loginVm);

	[Get("/api/cart?company-id={companyId}")]
	Task<CartViewModel> GetCartDetails(string companyId);

	[Post("/api/cart/items")]
	Task<int> AddProduct([Body] int productId);

	[Delete("/api/cart/items/{productId}")]
	Task<int> DeleteProduct(int productId);

	[Post("/api/customer/order/confirm")]
	Task<int> ConfirmOrder([Body] int cartId);

	[Patch("/api/customer/order/cancel")]
	Task CancelOrder([Body] int orderId);

	[Get("/api/customer/order?id={orderId}")]
	Task<OrderViewModel> OrderDetails(int orderId);

	[Get("/api/customers/me")]
	Task<CustomerInfoViewModel> GetCustomer();

	[Put("/api/customers/me/balance")]
	Task TopUpBalance(BalanceRequest balanceVm);

	[Post("/api/customers")]
	Task RegisterCustomer(RegisterRequest registerVm);
}
