using DynamicPriceClient.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers;
public class CartController : Controller
{
	private readonly ICoreApiClient _coreApiClient;

	public CartController(ICoreApiClient coreApiClient)
	{
		_coreApiClient = coreApiClient;
	}

	public async Task<IActionResult> Details(string companyId)
	{
		//todo: handle empty cart
		var cart = await _coreApiClient.GetCartDetails(companyId);
		return View(cart);
	}

	public async Task<IActionResult> AddProduct(int productId)
	{
		var companyId = await _coreApiClient.AddProduct(productId);
		return RedirectToAction(nameof(Details), new { companyId });
	}

	public async Task<IActionResult> RemoveProduct(int productId)
	{
		var companyId = await _coreApiClient.DeleteProduct(productId);
		return RedirectToAction(nameof(Details), new { companyId });
	}
}
