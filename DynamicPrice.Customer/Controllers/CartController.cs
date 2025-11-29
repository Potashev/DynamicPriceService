using DynamicPrice.Customer.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Customer.Controllers;

public class CartController : BaseController
{
	public CartController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Details(string companyId)
	{
		//todo: handle empty cart
		var cart = await CoreApiClient.GetCartDetails(companyId);
		return View(cart);
	}

	public async Task<IActionResult> AddProduct(int productId)
	{
		var companyId = await CoreApiClient.AddProduct(productId);
		return RedirectToAction(nameof(Details), new { companyId });
	}

	public async Task<IActionResult> RemoveProduct(int productId)
	{
		var companyId = await CoreApiClient.DeleteProduct(productId);
		return RedirectToAction(nameof(Details), new { companyId });
	}
}
