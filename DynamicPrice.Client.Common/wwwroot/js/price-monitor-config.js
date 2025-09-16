export class PriceMonitorConfig {
	constructor({
		maxPoints = 100,
		chartOptions = {},
		datasetOptions = {}
	} = {}) {
		this.maxPoints = maxPoints;
		this.chartOptions = chartOptions;
		this.datasetOptions = datasetOptions;
	}
}