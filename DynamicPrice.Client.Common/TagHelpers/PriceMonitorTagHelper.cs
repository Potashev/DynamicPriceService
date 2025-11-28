using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Json;

[HtmlTargetElement("price-monitor")]
public class PriceMonitorTagHelper : TagHelper
{
	[HtmlAttributeName("product-id")]
	public int ProductId { get; set; }

	[HtmlAttributeName("price")]
	public decimal Price { get; set; }

	[HtmlAttributeName("dynamics")]
	public IEnumerable<dynamic>? Dynamics { get; set; }

	[HtmlAttributeName("max-points")]
	public int? MaxPoints { get; set; }

	[HtmlAttributeName("show-axes")]
	public bool? ShowAxes { get; set; }

	[HtmlAttributeName("show-grid")]
	public bool? ShowGrid { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "div";
		output.Attributes.SetAttribute("class", "chart-container");

		var dynamicsJson = JsonSerializer.Serialize(
			Dynamics?.Select(d => new { date = d.Date, price = d.Price }) ?? Enumerable.Empty<object>()
		);

		var options = new
		{
			maxPoints = MaxPoints ?? 10,
			chartOptions = new
			{
				scales = new
				{
					y = new
					{
						display = ShowAxes ?? false,
						grid = new { display = ShowGrid ?? false }
					},
					x = new
					{
						display = ShowAxes ?? false,
						grid = new { display = ShowGrid ?? false }
					}
				}
			}
		};

		var optionsJson = JsonSerializer.Serialize(options);

		output.Content.SetHtmlContent($@"
			<canvas id=""chart-{ProductId}""
					data-price-monitor=""true""
					data-product-id=""{ProductId}""
					data-initial-data='{dynamicsJson}'
					data-options='{optionsJson}'></canvas>
		");
	}
}