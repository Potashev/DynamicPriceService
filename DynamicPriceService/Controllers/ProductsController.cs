using DynamicPriceService.ApiClients;
using DynamicPriceService.Services;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;

public class ProductsController : Controller
{
	//private readonly HttpClientService _httpClientService;
	private readonly ICoreApiClient _coreApiClient;

	public ProductsController(ICoreApiClient coreApiClient)
		=> _coreApiClient = coreApiClient;

	public async Task<IActionResult> Index()
	{
		//var productsVm = await _httpClientService.GetAsync<IEnumerable<ProductViewModel>>("api/company/products");
		var productsVm = await _coreApiClient.GetProducts();
		return View(productsVm);
	}

	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var productVm = await _coreApiClient.GetProduct((int)id);
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
			//await _httpClientService.PostAsync("api/company/products", productVm);
			await _coreApiClient.CreateProduct(productVm);
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
		var productVm = await _coreApiClient.GetProduct((int)id);
		return View(productVm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, ProductViewModel productVm)	//todo: looks not good
	{
		if (ModelState.IsValid)
		{
			//await _httpClientService.PutAsync($"api/company/products/{id}", productVm);
			await _coreApiClient.UpdateProduct(id, productVm);
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

		var productVm = await _coreApiClient.GetProduct((int)id);
		if (productVm == null)
		{
			return NotFound();
		}
		return View(productVm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		await _coreApiClient.DeleteProduct((int)id);
		return RedirectToAction(nameof(Index));
	}
}
