using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Json;

namespace DynamicPrice.Client.Common.TagHelpers
{
	[HtmlTargetElement("price-monitor")]
	public class PriceMonitorTagHelper : TagHelper
	{
		[HtmlAttributeName("product-id")]
		public int ProductId { get; set; }

		[HtmlAttributeName("price")]
		public decimal Price { get; set; }

		[HtmlAttributeName("dynamics")]
		public IEnumerable<object>? Dynamics { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "div"; // оборачиваем всё в контейнер
			output.Attributes.SetAttribute("class", "price-monitor");

			var dynamicsJson = JsonSerializer.Serialize(Dynamics ?? Enumerable.Empty<object>());

			output.Content.SetHtmlContent($@"
                <div class=""chart-container"">
                    <canvas id=""chart-{ProductId}""
                            data-price-monitor=""true""
                            data-product-id=""{ProductId}""
                            data-price=""{Price}""
                            data-initial-data='{dynamicsJson}'></canvas>
                </div>
            ");
		}
	}
}
