using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DynamicPrice.Client.Infrastructure.TagHelpers;

[HtmlTargetElement("error-alert")]
public class ErrorAlertTagHelper : TagHelper
{
	[ViewContext]
	[HtmlAttributeNotBound]
	public ViewContext ViewContext { get; set; } = default!;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		var error = ViewContext.TempData["Error"]?.ToString();

		if (string.IsNullOrEmpty(error))
		{
			output.SuppressOutput();
			return;
		}

		output.TagName = "div";
		output.TagMode = TagMode.StartTagAndEndTag;
		output.Attributes.SetAttribute("class", "alert alert-danger alert-dismissible fade show");
		output.Attributes.SetAttribute("role", "alert");

		output.Content.SetHtmlContent($@"
			<div>{error}</div>
			<button type=""button"" class=""btn-close"" data-bs-dismiss=""alert""></button>
		");
	}
}