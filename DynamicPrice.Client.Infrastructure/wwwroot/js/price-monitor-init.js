import { PriceMonitor } from './price-monitor.js';

const defaultHubUrl = "https://localhost:7140/priceHub"; // fallback

function resolveHubUrlFromDom() {
	// priority: body.dataset.priceHubUrl -> first canvas[data-price-hub-url] -> default
	const bodyUrl = document.body?.dataset?.priceHubUrl;
	if (bodyUrl) return bodyUrl;

	const el = document.querySelector("canvas[data-price-hub-url]");
	if (el) return el.dataset.priceHubUrl;

	return defaultHubUrl;
}

function resolveCompanyIdFromDom() {
	// priority: body.dataset.companyId -> first canvas[data-company-id] -> null
	const bodyCompany = document.body?.dataset?.companyId;
	if (bodyCompany) return Number(bodyCompany);

	const el = document.querySelector("canvas[data-company-id]");
	if (el) return Number(el.dataset.companyId);

	return null;
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

// Subscribe to company once (if available)
const companyId = resolveCompanyIdFromDom();
if (companyId) {
	try {
		await monitor.subscribeToCompany(companyId);
	} catch (e) {
		console.error("subscribeToCompany failed", companyId, e);
	}
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

	// subscribe to product-level group for more targeted updates
	(async () => {
		try {
			await monitor.subscribeToProduct(Number(productId));
		} catch (e) {
			console.debug("subscribeToProduct failed for", productId, e);
		}
	})();
});
