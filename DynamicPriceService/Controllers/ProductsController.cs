using DynamicPriceService.Services;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;

public class ProductsController : Controller
{
	private readonly HttpClientService _httpClientService;

	public ProductsController(HttpClientService httpClientService)
	{
		_httpClientService = httpClientService;
	}

	public async Task<IActionResult> Index()
	{
		var productsVm = await _httpClientService.GetAsync<IEnumerable<ProductViewModel>>("api/company/products");
		return View(productsVm);
	}

	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var productVm = await _httpClientService.GetAsync<ProductViewModel>($"api/company/products/{id}");
		return View(productVm);
	}

	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(ProductViewModel productVm)
	{
		if (ModelState.IsValid)
		{
			await _httpClientService.PostAsync("api/company/products", productVm);
			return RedirectToAction(nameof(Index));
		}
		return View(productVm);
	}

	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var productVm = await _httpClientService.GetAsync<ProductViewModel>($"api/company/products/{id}");
		return View(productVm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, ProductViewModel productVm)
	{
		if (ModelState.IsValid)
		{
			await _httpClientService.PutAsync($"api/company/products/{id}", productVm);
			return RedirectToAction(nameof(Index));
		}
		return View(productVm);
	}

	public async Task<IActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}

		var productVm = await _httpClientService.GetAsync<ProductViewModel>($"api/company/products/{id}");
		if (productVm == null)
		{
			return NotFound();
		}
		return View(productVm);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		await _httpClientService.DeleteAsync($"api/company/products/{id}");
		return RedirectToAction(nameof(Index));
	}
}
