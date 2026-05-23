using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CartController(
	ICoreApiClient coreApiClient) 
	: Controller
{
	[HttpGet]
	public async Task<IActionResult> Details(
		Guid companyId,
		CancellationToken cancellationToken)
			=> View(await coreApiClient.GetCartDetails(companyId, cancellationToken));

	[HttpPost]
	public async Task<IActionResult> AddCartItem(
		Guid productId,
		CancellationToken cancellationToken)
	{
		var companyId = await coreApiClient.AddCartItem(productId, cancellationToken);
		return RedirectToAction(nameof(Details), new { companyId });
	}

	[HttpPost]
	public async Task<IActionResult> RemoveCartItem(
		Guid productId,
		CancellationToken cancellationToken)
	{
		var companyId = await coreApiClient.RemoveCartItem(productId, cancellationToken);
		return RedirectToAction(nameof(Details), new { companyId });
	}
}
