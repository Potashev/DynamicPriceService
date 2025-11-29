using DynamicPrice.Client.Common;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DynamicPrice.Client.TagHelpers
{
	[HtmlTargetElement("order-status")]
	public class OrderStatusTagHelper : TagHelper
	{
		public OrderStatus Status { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "span";

			string cssClass = Status switch
			{
				OrderStatus.Completed => "text-success",
				OrderStatus.Canceled => "text-danger",
				_ => ""
			};

			if (!string.IsNullOrEmpty(cssClass))
			{
				output.Attributes.SetAttribute("class", cssClass);
			}

			output.Content.SetContent(Status.ToString());
		}
	}
}