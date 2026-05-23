using DynamicPrice.Shared.Contracts.Requests;
using DynamicPrice.Shared.Contracts.Responses;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using Refit;

namespace DynamicPrice.Customer.ApiClients;

public interface ICoreApiClient
{
	[Get("/api/companies?status=active")]
	Task<IEnumerable<CompanyViewModel>> GetCompanies(
		CancellationToken cancellationToken);

	[Get("/api/companies/{companyId}/products")]
	Task<CompanyProductsInfo> GetCompanyProducts(
		Guid companyId,
		CancellationToken cancellationToken);

	[Post("/api/auth/login")]
	Task<TokenResponse> LoginCustomer(
		LoginRequest loginVm,
		CancellationToken cancellationToken);

	[Get("/api/cart?company-id={companyId}")]
	Task<CartViewModel> GetCartDetails(
		Guid companyId,
		CancellationToken cancellationToken);

	[Post("/api/cart/items")]
	Task<int> AddCartItem(
		[Body] Guid productId,
		CancellationToken cancellationToken);

	[Delete("/api/cart/items/{productId}")]
	Task<int> RemoveCartItem(
		Guid productId,
		CancellationToken cancellationToken);

	[Post("/api/customer/order/confirm")]
	Task<int> ConfirmOrder(
		[Body] Guid cartId,
		CancellationToken cancellationToken);

	[Patch("/api/customer/order/cancel")]
	Task CancelOrder(
		[Body] Guid orderId,
		CancellationToken cancellationToken);

	[Get("/api/customer/order?id={orderId}")]
	Task<OrderViewModel> OrderDetails(
		Guid orderId,
		CancellationToken cancellationToken);

	[Get("/api/customers/me")]
	Task<CustomerInfoViewModel> GetCustomer(
		CancellationToken cancellationToken = default);

	[Put("/api/customers/me/balance")]
	Task TopUpBalance(
		BalanceRequest balanceVm,
		CancellationToken cancellationToken);

	[Post("/api/customers")]
	Task RegisterCustomer(
		RegisterRequest registerVm,
		CancellationToken cancellationToken);
}
