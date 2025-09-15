import { PriceMonitorConfig } from './price-monitor-config.js';

export class PriceMonitor {
	constructor(hubUrl, config = new PriceMonitorConfig()) {
		this.hubUrl = hubUrl;
		this.config = config;
		this.connection = null;
		this.charts = {};
	}

	async init() {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(this.hubUrl)
			.build();

		this.connection.on("ReceivePriceUpdate", (productId, newPrice) => {
			this.updateChart(productId, newPrice);
			this.updatePriceLabel(productId, newPrice);
		});

		await this.connection.start();
		console.log("PriceMonitor connected to SignalR");
	}

	registerChart(productId, canvasId, initialData = []) {
		// Обрезаем данные сразу при инициализации
		if (initialData.length > this.config.maxPoints) {
			initialData = initialData.slice(-this.config.maxPoints);
		}

		const ctx = document.getElementById(canvasId).getContext("2d");
		const chart = new Chart(ctx, {
			type: 'line',
			data: {
				labels: initialData.map((_, i) => `T${i + 1}`),
				datasets: [{
					label: 'Price Change',
					data: initialData,
					borderColor: 'rgba(75, 192, 192, 1)',
					fill: true,
					pointRadius: 1,
					...this.config.datasetOptions
				}]
			},
			options: {
				scales: { x: { display: false }, y: { display: false } },
				plugins: { legend: { display: false } },
				...this.config.chartOptions
			}
		});
		this.charts[productId] = chart;
	}

	updateChart(productId, newPrice) {
		const chart = this.charts[productId];
		if (!chart) return;

		// Сначала удаляем лишние точки (если их уже maxPoints)
		if (chart.data.datasets[0].data.length >= this.config.maxPoints) {
			chart.data.datasets[0].data.shift();
			chart.data.labels.shift();
		}

		// Потом добавляем новую точку
		chart.data.datasets[0].data.push(newPrice);
		chart.data.labels.push(new Date().toLocaleTimeString());

		chart.update();
	}

	updatePriceLabel(productId, newPrice) {
		const el = document.getElementById(`price-${productId}`);
		if (el) el.innerText = newPrice.toFixed(2);
	}
}
