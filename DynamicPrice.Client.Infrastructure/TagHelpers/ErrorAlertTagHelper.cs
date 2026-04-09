using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DynamicPrice.Client.Infrastructure.TagHelpers;

[HtmlTargetElement("error-alert")]
public class ErrorAlertTagHelper(
	IHtmlGenerator generator) : TagHelper
{
	[ViewContext]
	[HtmlAttributeNotBound]
	public ViewContext ViewContext { get; set; } = default!;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		var tempData = ViewContext.TempData;
		var modelState = ViewContext.ModelState;

		var hasTempError = tempData["Error"] != null;
		var hasModelErrors = !modelState.IsValid;

		if (!hasTempError && !hasModelErrors)
		{
			output.SuppressOutput();
			return;
		}

		output.TagName = "div";
		output.TagMode = TagMode.StartTagAndEndTag;
		output.Attributes.SetAttribute("class", "alert alert-danger alert-dismissible fade show");
		output.Attributes.SetAttribute("role", "alert");

		if (hasTempError)
		{
			output.Content.AppendHtml($"<div>{tempData["Error"]}</div>");
		}

		if (hasModelErrors)
		{
			var validationSummary = generator.GenerateValidationSummary(
				ViewContext,
				false,
				null,
				null,
				null
			);

			output.Content.AppendHtml(validationSummary);
		}

		output.Content.AppendHtml(
			"<button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\"></button>"
		);
	}
}