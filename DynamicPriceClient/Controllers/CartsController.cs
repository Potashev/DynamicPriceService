using DynamicPriceClient.ApiClients;
using DynamicPriceClient.Services;
using DynamicPriceClient.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers;
public class CartsController : Controller
{
	//private readonly HttpClientService _httpClientService;
	private readonly ICoreApiClient _coreApiClient;

	public CartsController(ICoreApiClient coreApiClient)
	{
		_coreApiClient = coreApiClient;
	}

	public async Task<IActionResult> CartDetails(string companyId)
	{
		//todo: handle empty cart
		//var cart = await _httpClientService.GetAsync<CartViewModel>($"api/carts/{companyId}");
		var cart = await _coreApiClient.GetCartDetails(companyId);
		return View(cart);
	}

	public async Task<IActionResult> AddProduct(int productId)
	{
		//var companyId = await _httpClientService.PostAsync<int, int>("api/carts/items", productId);
		var companyId = await _coreApiClient.AddProduct(productId);
		return RedirectToAction(nameof(CartDetails), new { companyId });
	}

	public async Task<IActionResult> RemoveProduct(int productId)
	{
		//var companyId = await _httpClientService.DeleteAsync<int>($"api/carts/items/{productId}");
		var companyId = await _coreApiClient.DeleteProduct(productId);
		return RedirectToAction(nameof(CartDetails), new { companyId });
	}

	public async Task<IActionResult> ConfirmOrder(int cartId)
	{
		//var receiveKey = await _httpClientService.PostAsync<int, int>("api/orders", cartId);
		var receiveKey = await _coreApiClient.ConfirmOrder(cartId);
		return Content($"Your receive Key: {receiveKey}");
	}

}
