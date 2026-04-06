using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CartController : BaseController
{
	public CartController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Details(
		string companyId,
		CancellationToken cancellationToken)
			=> View(await CoreApiClient.GetCartDetails(companyId, cancellationToken));

	public async Task<IActionResult> AddCartItem(
		int productId,
		CancellationToken cancellationToken)
	{
		var companyId = await CoreApiClient.AddCartItem(productId, cancellationToken);	//todo: redirect when throw?
		return RedirectToAction(nameof(Details), new { companyId });
	}

	public async Task<IActionResult> RemoveCartItem(
		int productId,
		CancellationToken cancellationToken)
	{
		var companyId = await CoreApiClient.RemoveCartItem(productId, cancellationToken); //todo: redirect when throw?
		return RedirectToAction(nameof(Details), new { companyId });
	}
}
