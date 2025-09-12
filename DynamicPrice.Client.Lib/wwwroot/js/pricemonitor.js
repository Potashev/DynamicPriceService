window.PriceMonitor = (function () {
	let connection;
	let charts = {};
	let config = {};

	async function init(userConfig) {
		config = userConfig;

		connection = new signalR.HubConnectionBuilder()
			.withUrl(config.hubUrl)
			.build();

		connection.on("ReceivePriceUpdate", (productId, newPrice) => {
			updateChart(productId, newPrice);
			updatePriceLabel(productId, newPrice);
		});

		try {
			await connection.start();
			console.log("[PriceMonitor] Connected to SignalR hub.");
		} catch (err) {
			console.error("[PriceMonitor] SignalR connection error:", err);
		}
	}

	function updateChart(productId, newPrice) {
		const chart = charts[productId];
		if (!chart) return;

		chart.data.datasets[0].data.push(newPrice);
		chart.data.labels.push(new Date().toLocaleTimeString());

		if (chart.data.datasets[0].data.length > 100) {
			chart.data.datasets[0].data.shift();
			chart.data.labels.shift();
		}

		chart.update();
	}

	function updatePriceLabel(productId, newPrice) {
		const el = document.getElementById('price-' + productId);
		if (el) el.innerText = newPrice;
	}

	function registerChart(productId, initialData) {
		const ctx = document.getElementById('chart-' + productId).getContext('2d');
		const prices = (config.historyDays > 0)
			? initialData.filter(d => isWithinDays(d.date, config.historyDays)).map(d => d.price)
			: initialData.map(d => d.price);

		charts[productId] = new Chart(ctx, {
			type: 'line',
			data: {
				labels: Array.from({ length: prices.length }, (_, i) => `T${i + 1}`),
				datasets: [{
					label: 'Price Change',
					data: prices,
					borderColor: 'rgba(75, 192, 192, 1)',
					borderWidth: 2,
					fill: true,
					pointRadius: 1
				}]
			},
			options: {
				scales: { x: { display: false }, y: { display: false } },
				plugins: { legend: { display: false } },
				animation: { duration: 300, easing: 'linear' }
			}
		});
	}

	function isWithinDays(dateString, days) {
		const date = new Date(dateString);
		const cutoff = new Date();
		cutoff.setDate(cutoff.getDate() - days);
		return date >= cutoff;
	}

	return { init, registerChart };
})();
