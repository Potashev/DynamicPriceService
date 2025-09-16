import { PriceMonitor } from './price-monitor.js';
import { PriceMonitorConfig } from './price-monitor-config.js';

const config = new PriceMonitorConfig({
	maxPoints: 50,
	chartOptions: {
		scales: { x: { display: false }, y: { display: false } }
	}
});

const monitor = new PriceMonitor("https://localhost:7140/priceHub", config);	//todo: get url from config
await monitor.init();

// ищем все элементы с data-price-monitor
document.querySelectorAll("[data-price-monitor]").forEach(el => {
	const productId = el.dataset.productId;
	const data = JSON.parse(el.dataset.initialData || "[]");

	monitor.registerChart(
		productId,
		el.id || `chart-${productId}`,
		data
	);
});
