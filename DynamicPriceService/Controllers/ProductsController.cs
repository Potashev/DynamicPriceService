using DynamicPrice.Manager.Controllers;
using DynamicPriceService.ApiClients;
using DynamicPriceService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceService.Controllers;

public class ProductsController : BaseController
{
	public ProductsController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Index()
	{
		var productsVm = await CoreApiClient.GetProducts();
		return View(productsVm);
	}

	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var productVm = await CoreApiClient.GetProduct((int)id);
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
			await CoreApiClient.CreateProduct(productVm);
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
		var productVm = await CoreApiClient.GetProduct((int)id);
		return View(productVm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, ProductViewModel productVm)   //todo: looks not good
	{
		if (ModelState.IsValid)
		{
			await CoreApiClient.UpdateProduct(id, productVm);
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

		var productVm = await CoreApiClient.GetProduct((int)id);
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
		await CoreApiClient.DeleteProduct(id);
		return RedirectToAction(nameof(Index));
	}
}
