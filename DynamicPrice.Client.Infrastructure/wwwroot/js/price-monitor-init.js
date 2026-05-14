import { PriceMonitor } from './price-monitor.js';

const defaultHubUrl = "http://localhost:5001/priceHub";

function resolveHubUrlFromDom() {
	// priority: body.dataset.priceHubUrl -> first canvas[data-price-hub-url] -> default
	const bodyUrl = document.body?.dataset?.priceHubUrl;
	if (bodyUrl) return bodyUrl;

	const el = document.querySelector("canvas[data-price-hub-url]");
	if (el) return el.dataset.priceHubUrl;

	return defaultHubUrl;
}

const hubUrl = resolveHubUrlFromDom();
const monitor = new PriceMonitor(hubUrl);

try {
	await monitor.init();
	window.priceMonitorInstance = monitor;
	window.dispatchEvent(new CustomEvent('priceMonitorReady'));
	console.log("PriceMonitor initialized, hubUrl:", hubUrl);
} catch (err) {
	console.error("PriceMonitor init failed", err, "hubUrl:", hubUrl);
}

// register charts and subscribe to product groups
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

	// subscribe to product group for targeted updates
	(async () => {
		try {
			await monitor.subscribeToProduct(Number(productId));
		} catch (e) {
			console.debug("subscribeToProduct failed for", productId, e);
		}
	})();
});

// try to unsubscribe on unload (best-effort)
window.addEventListener('beforeunload', () => {
	if (window.priceMonitorInstance && typeof window.priceMonitorInstance.unsubscribeAll === 'function') {
		// best-effort synchronous attempt: fire-and-forget
		window.priceMonitorInstance.unsubscribeAll();
	}
});
