import { PriceMonitor } from './price-monitor.js';
/*import { PriceMonitorConfig } from './price-monitor-config.js';*/

const hubUrl = "/priceHub"; // можно вынести в конфиг / data-атрибут
const monitor = new PriceMonitor(hubUrl);

// инициализируем и делаем глобально доступным экземпляр
await monitor.init();
window.priceMonitorInstance = monitor;
// оповещаем внешние скрипты, что monitor готов
window.dispatchEvent(new CustomEvent('priceMonitorReady'));

console.log("PriceMonitor initialized and exported as window.priceMonitorInstance");

// если на странице задан companyId в data атрибуте body, автоматически подпишемся
const companyId = document.body.dataset.companyId;
if (companyId) {
	try {
		await monitor.subscribeToCompany(Number(companyId));
	} catch (e) {
		console.error("Failed to subscribe to company", companyId, e);
	}
}

// ищем все элементы с data-price-monitor и регистрируем графики
document.querySelectorAll("[data-price-monitor]").forEach(el => {
	const productId = el.dataset.productId;
	const data = JSON.parse(el.dataset.initialData || "[]");
	const options = el.dataset.options ? JSON.parse(el.dataset.options) : {};

	monitor.registerChart(
		productId,
		el.id || `chart-${productId}`,
		data,
		options
	);

	// опционально подпишемся на конкретный продукт (например для product.details это полезно)
	(async () => {
		try {
			await monitor.subscribeToProduct(Number(productId));
		} catch (e) {
			// если нет прав или метод на hub недоступен — просто логируем
			console.debug("subscribeToProduct failed for", productId, e);
		}
	})();
});
