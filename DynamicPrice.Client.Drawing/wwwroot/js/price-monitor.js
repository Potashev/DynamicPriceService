export class PriceMonitor {
	constructor(hubUrl) {
		this.hubUrl = hubUrl;
		this.connection = null;
		this.charts = {};
	}

	async init() {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(this.hubUrl)
			.build();

		// Обработчик получения новых цен
		this.connection.on("ReceivePriceUpdate", (productId, newPrice) => {
			this.updateChart(productId, newPrice);
			this.updatePriceLabel(productId, newPrice); // <--- обновление цены
		});

		await this.connection.start();
		console.log("PriceMonitor connected to SignalR");
	}

	registerChart(productId, canvasId, initialData = []) {
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
					pointRadius: 1
				}]
			},
			options: {
				scales: { x: { display: false }, y: { display: false } },
				plugins: { legend: { display: false } }
			}
		});
		this.charts[productId] = chart;
	}

	updateChart(productId, newPrice) {
		const chart = this.charts[productId];
		if (!chart) return;

		chart.data.datasets[0].data.push(newPrice);
		chart.data.labels.push(new Date().toLocaleTimeString());
		if (chart.data.datasets[0].data.length > 100) {
			chart.data.datasets[0].data.shift();
			chart.data.labels.shift();
		}
		chart.update();
	}

	updatePriceLabel(productId, newPrice) {
		const el = document.getElementById(`price-${productId}`);
		if (el) el.innerText = newPrice.toFixed(2); // можно форматировать
	}
}
