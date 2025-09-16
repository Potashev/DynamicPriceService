export class PriceMonitor {
	constructor(hubUrl) {
		this.hubUrl = hubUrl;
		this.config = {
			maxPoints: 100,
			chartOptions: {},
			datasetOptions: {}
		};
		this.connection = null;
		this.charts = {};
		this.lastPrices = {};
		this.chartMaxPoints = {}; // локальный maxPoints для каждого графика
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
		const maxPoints = options.maxPoints ?? this.config.maxPoints;

		// сохраняем maxPoints для этого графика
		this.chartMaxPoints[productId] = maxPoints;

		if (initialData.length > maxPoints) {
			initialData = initialData.slice(-maxPoints);
		}

		const labels = initialData.map(d => new Date(d.date).toLocaleTimeString());
		const prices = initialData.map(d => d.price);

		this.lastPrices[productId] = prices.length > 0 ? prices[prices.length - 1] : null;

		const ctx = document.getElementById(canvasId).getContext("2d");

		const localChartOptions = {
			...this.config.chartOptions,
			...(options.chartOptions || {})
		};

		const localDatasetOptions = {
			...this.config.datasetOptions,
			...(options.datasetOptions || {})
		};

		const chart = new Chart(ctx, {
			type: 'line',
			data: {
				labels: labels,
				datasets: [{
					//label: 'Price Change',
					data: prices,
					borderColor: 'rgba(75, 192, 192, 1)',
					fill: true,
					pointRadius: 1,
					...localDatasetOptions
				}]
			},
			options: {
				//animation: false,
				responsive: true,
				plugins: {
					legend: { display: false },
					//tooltip: { enabled: false }
				},
				scales: {
					x: { display: false },
					y: { display: false }
				},
				...localChartOptions
			}
		});

		this.charts[productId] = chart;
	}

	updateChart(productId, newPrice) {
		const chart = this.charts[productId];
		if (!chart) return;

		const maxPoints = this.chartMaxPoints[productId] ?? this.config.maxPoints;

		if (chart.data.datasets[0].data.length >= maxPoints) {
			chart.data.datasets[0].data.shift();
			chart.data.labels.shift();
		}

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

		const diff = newPrice - prevPrice;
		const percentChange = (diff / prevPrice) * 100;

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
