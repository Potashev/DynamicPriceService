import { PriceMonitor } from './price-monitor.js';

const defaultHubUrl = "https://localhost:7140/priceHub"; // замените на адрес вашего Core сервиса при необходимости

// используем data-атрибут body.dataset.priceHubUrl, если он задан на странице
const hubUrl = document.body?.dataset?.priceHubUrl || defaultHubUrl;

const monitor = new PriceMonitor(hubUrl);
try {
	await monitor.init();
	window.priceMonitorInstance = monitor;
	window.dispatchEvent(new CustomEvent('priceMonitorReady'));
	console.log("PriceMonitor initialized and exported as window.priceMonitorInstance, hubUrl:", hubUrl);
} catch (err) {
	console.error("PriceMonitor init failed", err, "hubUrl:", hubUrl);
}

// если на странице задан companyId в data атрибуте body, автоматически подпишемся
const companyId = document.body?.dataset?.companyId;
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

	// опционально подпишемся на конкретный продукт
	(async () => {
		try {
			await monitor.subscribeToProduct(Number(productId));
		} catch (e) {
			console.debug("subscribeToProduct failed for", productId, e);
		}
	})();
});
