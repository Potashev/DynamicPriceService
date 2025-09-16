using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DynamicPrice.Client.Common.TagHelpers
{
	[HtmlTargetElement("price-monitor-resources")]
	public class PriceMonitorResourcesTagHelper : TagHelper
	{
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null; // самозакрывающийся тег, ничего лишнего
			output.Content.SetHtmlContent($@"
                <script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
                <script src=""https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.0/signalr.min.js""></script>
                <link rel=""stylesheet"" href=""/_content/DynamicPrice.Client.Common/css/price-monitor.css"" />
                <script type=""module"" src=""/_content/DynamicPrice.Client.Common/js/price-monitor-init.js""></script>
            ");
		}
	}
}
