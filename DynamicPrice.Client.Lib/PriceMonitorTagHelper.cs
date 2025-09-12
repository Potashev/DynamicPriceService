using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DynamicPrice.Client.Common;

[HtmlTargetElement("price-monitor")]
public class PriceMonitorTagHelper : TagHelper
{
	/// <summary>
	/// URL SignalR-хаба (по умолчанию /priceHub)
	/// </summary>
	public string HubUrl { get; set; } = "/priceHub";

	/// <summary>
	/// Количество дней истории для отображения (N). Если 0 – не фильтруем.
	/// </summary>
	public int HistoryDays { get; set; } = 0;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "div"; // основной контейнер
		output.Attributes.SetAttribute("id", "price-monitor-root");

		// Подключаем SignalR, Chart.js и локальный JS из wwwroot веб-проекта
		output.PostContent.AppendHtml($@"
<script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
<script src=""https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.0/signalr.min.js""></script>
<script src=""/js/pricemonitor.js""></script>
<script>
    window.PriceMonitor.init({{
        hubUrl: '{HubUrl}',
        historyDays: {HistoryDays}
    }});
</script>
");
	}

	//	public override void Process(TagHelperContext context, TagHelperOutput output)
	//	{
	//		output.TagName = "div"; // основной контейнер
	//		output.Attributes.SetAttribute("id", "price-monitor-root");

	//		// Подключаем SignalR, Chart.js и наш кастомный JS из RCL
	//		output.PostContent.AppendHtml($@"
	//<script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
	//<script src=""https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.0/signalr.min.js""></script>
	//<script src=""/dp-common/js/pricemonitor.js""></script>
	//<script>
	//    window.PriceMonitor.init({{
	//        hubUrl: '{HubUrl}',
	//        historyDays: {HistoryDays}
	//    }});
	//</script>
	//");
	//	}



}
