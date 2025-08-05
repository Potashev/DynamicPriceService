using DynamicPriceClient.Services;
using DynamicPriceClient.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceClient.Controllers;
public class CartsController : Controller
{
	private readonly HttpClientService _httpClientService;

	public CartsController(HttpClientService httpClientService)
	{
		_httpClientService = httpClientService;
	}

	public async Task<IActionResult> CartDetails(string companyId)
	{
		//todo: handle empty cart
		var cart = await _httpClientService.GetAsync<CartViewModel>($"api/carts/{companyId}");
		return View(cart);
	}

	public async Task<IActionResult> AddProduct(int productId)
	{
		var companyId = await _httpClientService.PostAsync<int, int>("api/carts/items", productId);
		return RedirectToAction(nameof(CartDetails), new { companyId });
	}

	public async Task<IActionResult> RemoveProduct(int productId)
	{
		var companyId = await _httpClientService.DeleteAsync<int>($"api/carts/items/{productId}");
		return RedirectToAction(nameof(CartDetails), new { companyId });
	}

	public async Task<IActionResult> ConfirmOrder(int cartId)
	{
		var receiveKey = await _httpClientService.PostAsync<int, int>("api/orders", cartId);
		return Content($"Your receive Key: {receiveKey}");
	}

}
