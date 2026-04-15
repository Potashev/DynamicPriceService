using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class ProductsController(
	ICoreApiClient coreApiClient) 
	: Controller
{
	[HttpGet]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await coreApiClient.GetProducts(cancellationToken));

	[HttpGet]
	public async Task<IActionResult> Details(
		int? id,
		CancellationToken cancellationToken)
	{
		if (id is null)
			return NotFound();

		var productVm = await coreApiClient.GetProduct((int)id, cancellationToken);
		return View(productVm);
	}

	[HttpGet]
	public IActionResult Create()
		=> View();

	[HttpPost]
	public async Task<IActionResult> Create(
		ProductViewModel productVm,
		CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			await coreApiClient.CreateProduct(productVm, cancellationToken);
			return RedirectToAction(nameof(Index));
		}
		return View(productVm);
	}

	[HttpGet]
	public async Task<IActionResult> Edit(
		int? id,
		CancellationToken cancellationToken)
	{
		if (id is null) return NotFound();

		var productVm = await coreApiClient.GetProduct((int)id, cancellationToken);
		return View(productVm);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(
		ProductViewModel productVm,
		CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			await coreApiClient.UpdateProduct(productVm, cancellationToken);
			return RedirectToAction(nameof(Index));
		}
		return View(productVm);
	}

	[HttpGet]
	public async Task<IActionResult> Delete(
		int? id,
		CancellationToken cancellationToken)
	{
		if (id is null) return NotFound();

		var productVm = await coreApiClient.GetProduct((int)id, cancellationToken);
		return productVm == null
			? NotFound()
			: View(productVm);
	}

	[HttpPost]
	public async Task<IActionResult> DeleteConfirmed(
		int id,
		CancellationToken cancellationToken)
	{
		await coreApiClient.DeleteProduct(id, cancellationToken);
		return RedirectToAction(nameof(Index));
	}
}
