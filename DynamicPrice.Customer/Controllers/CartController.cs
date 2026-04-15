using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CartController(
	ICoreApiClient coreApiClient) 
	: Controller
{
	[HttpGet]
	public async Task<IActionResult> Details(
		string companyId,
		CancellationToken cancellationToken)
			=> View(await coreApiClient.GetCartDetails(companyId, cancellationToken));

	[HttpPost]
	public async Task<IActionResult> AddCartItem(
		int productId,
		CancellationToken cancellationToken)
	{
		var companyId = await coreApiClient.AddCartItem(productId, cancellationToken);
		return RedirectToAction(nameof(Details), new { companyId });
	}

	[HttpPost]
	public async Task<IActionResult> RemoveCartItem(
		int productId,
		CancellationToken cancellationToken)
	{
		var companyId = await coreApiClient.RemoveCartItem(productId, cancellationToken);
		return RedirectToAction(nameof(Details), new { companyId });
	}
}
