using DynamicPrice.Shared.Contracts.Requests;
using DynamicPrice.Shared.Contracts.Responses;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using Refit;

namespace DynamicPrice.Manager.ApiClients;

public interface ICoreApiClient
{
	[Post("/api/auth/login")]
	Task<TokenResponse> LoginManager(
		LoginRequest loginVm,
		CancellationToken cancellationToken);

	[Get("/api/company/managers")]
	Task<IEnumerable<ManagerInfoViewModel>> GetCompanyManagers(
		CancellationToken cancellationToken);

	[Post("/api/company/managers")]
	Task RegisterManager(
		RegisterRequest registerVm,
		CancellationToken cancellationToken);

	[Get("/api/company/managers/me")]
	Task<ManagerInfoViewModel> GetManager(
		CancellationToken cancellationToken = default);

	[Get("/api/company/products")]
	Task<IEnumerable<ProductViewModel>> GetProducts(
		CancellationToken cancellationToken);

	[Get("/api/company/products/{id}")]
	Task<ProductViewModel> GetProduct(
		int id,
		CancellationToken cancellationToken);

	[Post("/api/company/products")]
	Task CreateProduct(
		ProductViewModel productVm,
		CancellationToken cancellationToken);

	[Put("/api/company/products/{productVm.ProductId}")]
	Task UpdateProduct(
		ProductViewModel productVm,
		CancellationToken cancellationToken);

	[Delete("/api/company/products/{id}")]
	Task DeleteProduct(
		int id,
		CancellationToken cancellationToken);

	[Get("/api/company/price-rule")]
	Task<PriceRuleWithStatus> GetPriceRule(
		CancellationToken cancellationToken);

	[Put("/api/company/price-rule")]
	Task UpdatePriceRule(
		PriceRuleViewModel priceRuleVm,
		CancellationToken cancellationToken);

	[Post("/api/company/price-rule/run")]
	Task RunPriceReducing(
		CancellationToken cancellationToken);

	[Post("/api/company/price-rule/stop")]
	Task StopPriceReducing(
		CancellationToken cancellationToken);

	[Get("/api/company/orders")]
	Task<IEnumerable<OrderViewModel>> GetOrders(
		CancellationToken cancellationToken);

	[Get("/api/company/orders/{id}")]
	Task<OrderViewModel> GetOrder(
		int id,
		CancellationToken cancellationToken);

	[Get("/api/company/orders/by-receive-key/{key}")]
	Task<int> GetOrderIdByReceiveKey(
		string key,
		CancellationToken cancellationToken);

	[Patch("/api/company/orders/{orderId}/ready")]
	Task ReadyForReceiveOrder(
		string orderId,
		CancellationToken cancellationToken);

	[Patch("/api/company/orders/{orderId}/complete")]
	Task CompleteOrder(
		string orderId,
		CancellationToken cancellationToken);

	[Get("/api/company/orders/statistics")]
	Task<OrdersStatistics> GetOrdersStatistics(
		CancellationToken cancellationToken);
}