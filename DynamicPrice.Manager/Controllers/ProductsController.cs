using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class ProductsController : BaseController
{
	public ProductsController(ICoreApiClient coreApiClient)
		: base(coreApiClient) { }

	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await CoreApiClient.GetProducts(cancellationToken));

	public async Task<IActionResult> Details(
		int? id,
		CancellationToken cancellationToken)
	{
		if (id is null)
			return NotFound();

		var productVm = await CoreApiClient.GetProduct((int)id, cancellationToken);
		return View(productVm);
	}

	public IActionResult Create()
		=> View();

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(
		ProductViewModel productVm,
		CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			await CoreApiClient.CreateProduct(productVm, cancellationToken);
			return RedirectToAction(nameof(Index));
		}
		return View(productVm);
	}

	public async Task<IActionResult> Edit(
		int? id,
		CancellationToken cancellationToken)
	{
		if (id == null)
		{
			return NotFound();
		}
		var productVm = await CoreApiClient.GetProduct((int)id, cancellationToken);
		return View(productVm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(
		ProductViewModel productVm,
		CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			await CoreApiClient.UpdateProduct(productVm, cancellationToken);
			return RedirectToAction(nameof(Index));
		}
		return View(productVm);
	}

	public async Task<IActionResult> Delete(
		int? id,
		CancellationToken cancellationToken)
	{
		if (id is null) return NotFound();

		var productVm = await CoreApiClient.GetProduct((int)id, cancellationToken);
		return productVm == null
			? NotFound()
			: View(productVm);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(
		int id,
		CancellationToken cancellationToken)
	{
		await CoreApiClient.DeleteProduct(id, cancellationToken);
		return RedirectToAction(nameof(Index));
	}
}
