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
		this.chartMaxPoints = {};

		// set of productIds we subscribed to on the hub
		this.subscribedProducts = new Set();

		this._unloadHandlerBound = false;
	}

	async init() {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(this.hubUrl)
			.build();

		this.connection.on("ReceivePriceUpdate", (productId, newPrice, dateUtc) => {
			this.updateChart(productId, newPrice, dateUtc);
			this.updatePriceLabel(productId, newPrice);
		});

		await this.connection.start();
		console.log("PriceMonitor connected to SignalR");

		// register best-effort unload handlers once
		if (!this._unloadHandlerBound) {
			this._bindUnloadHandlers();
			this._unloadHandlerBound = true;
		}
	}

	_bindUnloadHandlers() {
		// best-effort: try to unsubscribe on pagehide / beforeunload
		const tryUnsubscribe = () => {
			// fire-and-forget
			this.unsubscribeAll().catch(err => console.debug("unsubscribeAll failed", err));
		};
		window.addEventListener('pagehide', tryUnsubscribe);
		window.addEventListener('beforeunload', tryUnsubscribe);
	}

	// --- Subscription helpers ---
	async subscribeToProduct(productId) {
		if (!this.connection) throw new Error("SignalR connection not initialized");
		if (this.subscribedProducts.has(Number(productId))) {
			return;
		}
		try {
			await this.connection.invoke("SubscribeToProduct", Number(productId));
			this.subscribedProducts.add(Number(productId));
			console.log("Subscribed to product", productId);
		} catch (e) {
			console.error("Failed to subscribe to product", productId, e);
			throw e;
		}
	}

	async unsubscribeFromProduct(productId) {
		if (!this.connection) return;
		if (!this.subscribedProducts.has(Number(productId))) return;
		try {
			await this.connection.invoke("UnsubscribeFromProduct", Number(productId));
			this.subscribedProducts.delete(Number(productId));
			console.log("Unsubscribed from product", productId);
		} catch (e) {
			console.debug("Failed to unsubscribe from product", productId, e);
		}
	}

	async unsubscribeAll() {
		if (!this.connection) return;
		const ids = Array.from(this.subscribedProducts);
		if (ids.length === 0) return;
		// Try to unsubscribe in parallel but don't throw on failures
		await Promise.allSettled(ids.map(id => this.connection.invoke("UnsubscribeFromProduct", id)));
		this.subscribedProducts.clear();
		console.log("Unsubscribed from all products");
	}
	// --- /subscriptions ---

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

	//updateChart(productId, newPrice) {
	//	const chart = this.charts[productId];
	//	if (!chart) return;

	//	const maxPoints = this.chartMaxPoints[productId] ?? this.config.maxPoints;

	//	if (chart.data.datasets[0].data.length >= maxPoints) {
	//		chart.data.datasets[0].data.shift();
	//		chart.data.labels.shift();
	//	}

	//	chart.data.datasets[0].data.push(newPrice);
	//	chart.data.labels.push(new Date().toLocaleTimeString());

	//	chart.update();
	//}

	updateChart(productId, newPrice, dateUtc) {
		const chart = this.charts[productId];
		if (!chart) return;

		const maxPoints = this.chartMaxPoints[productId] ?? this.config.maxPoints;

		if (chart.data.datasets[0].data.length >= maxPoints) {
			chart.data.datasets[0].data.shift();
			chart.data.labels.shift();
		}

		// ❗ теперь используем UTC от сервера
		chart.data.datasets[0].data.push(newPrice);
		/*		chart.data.labels.push(formatUtcTime(dateUtc));*/
		chart.data.labels.push(dateUtc.toString());

		chart.update();
	}

	async formatUtcTime(dateUtc) {
	const d = new Date(dateUtc);

	return d.getUTCHours().toString().padStart(2, '0') + ':' +
		d.getUTCMinutes().toString().padStart(2, '0') + ':' +
		d.getUTCSeconds().toString().padStart(2, '0');
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
