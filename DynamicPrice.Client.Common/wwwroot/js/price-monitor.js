import { PriceMonitorConfig } from './price-monitor-config.js';

export class PriceMonitor {
	constructor(hubUrl, config = new PriceMonitorConfig()) {
		this.hubUrl = hubUrl;
		this.config = config;
		this.connection = null;
		this.charts = {};
		this.lastPrices = {}; // храним последнюю цену для каждого продукта
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

	registerChart(productId, canvasId, initialData = [], options = {}) {
		// Локальный maxPoints берём из options или глобального конфига
		const maxPoints = options.maxPoints ?? this.config.maxPoints;

		if (initialData.length > maxPoints) {
			initialData = initialData.slice(-maxPoints);
		}

		const labels = initialData.map(d => new Date(d.date).toLocaleTimeString());
		const prices = initialData.map(d => d.price);

		this.lastPrices[productId] = prices.length > 0 ? prices[prices.length - 1] : null;

		const ctx = document.getElementById(canvasId).getContext("2d");

		// Локальный конфиг для передачи в Chart.js
		const localConfig = new PriceMonitorConfig({
			maxPoints: maxPoints,
			chartOptions: {
				...this.config.chartOptions,
				...(options.chartOptions || {})
			},
			datasetOptions: {
				...this.config.datasetOptions,
				...(options.datasetOptions || {})
			}
		});

		const chart = new Chart(ctx, {
			type: 'line',
			data: {
				labels: labels,
				datasets: [{
					label: 'Price Change',
					data: prices,
					borderColor: 'rgba(75, 192, 192, 1)',
					fill: true,
					pointRadius: 1,
					...localConfig.datasetOptions
				}]
			},
			options: {
				scales: {
					x: { display: false },
					y: { display: false }
				},
				plugins: { legend: { display: false } },
				...localConfig.chartOptions
			}
		});

		this.charts[productId] = chart;
	}



	updateChart(productId, newPrice) {
		const chart = this.charts[productId];
		if (!chart) return;

		// удаляем лишние точки, если достигнут maxPoints
		if (chart.data.datasets[0].data.length >= this.config.maxPoints) {
			chart.data.datasets[0].data.shift();
			chart.data.labels.shift();
		}

		// добавляем новую точку
		chart.data.datasets[0].data.push(newPrice);
		chart.data.labels.push(new Date().toLocaleTimeString());

		chart.update();
	}

	updatePriceLabel(productId, newPrice) {
		const priceEl = document.getElementById(`price-${productId}`);
		const changeEl = document.getElementById(`price-change-${productId}`);

		if (priceEl) priceEl.innerText = newPrice.toFixed(2);

		const prevPrice = this.lastPrices[productId];
		this.lastPrices[productId] = newPrice;

		if (!changeEl || prevPrice === null || prevPrice === undefined) return;

		// вычисляем процент изменения
		const diff = newPrice - prevPrice;
		const percentChange = (diff / prevPrice) * 100;

		// очищаем классы перед установкой новых
		changeEl.classList.remove("price-up", "price-down");

		if (diff > 0) {
			changeEl.innerHTML = `<span class="price-up">▲ ${percentChange.toFixed(2)}%</span>`;
		} else if (diff < 0) {
			changeEl.innerHTML = `<span class="price-down">▼ ${Math.abs(percentChange).toFixed(2)}%</span>`;
		} else {
			changeEl.innerHTML = "";
		}
	}
}
