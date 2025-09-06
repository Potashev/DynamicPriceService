using DynamicPriceService.ViewModels;
using Refit;

namespace DynamicPriceService.ApiClients;

public interface ICoreApiClient
{
	[Post("/api/auth/login")]
	Task<TokenResponse> LoginManager(LoginViewModel loginVm);

	[Get("/api/company/products")]
	Task<IEnumerable<ProductViewModel>> GetProducts();

	[Get("/api/company/products/{id}")]
	Task<ProductViewModel> GetProduct(int id);

	[Post("/api/company/products")]
	Task CreateProduct(ProductViewModel productVm);

	[Put("/api/company/products/{id}")]
	Task UpdateProduct(int id, ProductViewModel productVm);

	[Delete("/api/company/products/{id}")]
	Task DeleteProduct(int id);

	[Get("/api/company/price-rule")]
	Task<PriceRuleWithStatus> GetPriceRule();

	[Put("/api/company/price-rule")]
	Task UpdatePriceRule(PriceRuleViewModel priceRuleVm);

	[Post("/api/company/price-rule/run")]
	Task RunPriceReducing();

	[Post("/api/company/price-rule/stop")]
	Task StopPriceReducing();

	[Get("/api/company/orders")]
	Task<IEnumerable<OrderViewModel>> GetOrders();

	[Get("/api/company/orders/{id}")]
	Task<OrderViewModel> GetOrder(int id);

	[Get("/api/company/orders/by-receive-key/{key}")]
	Task<int> GetOrderIdByReceiveKey(string key);

	[Patch("/api/company/orders/{orderId}/ready")]
	Task ReadyForReceiveOrder(string orderId);

	[Patch("/api/company/orders/{orderId}/complete")]
	Task CompleteOrder(string orderId);

	[Get("/api/company/orders/statistics")]
	Task<OrdersStatistics> GetOrdersStatistics();
}
