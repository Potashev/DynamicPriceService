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
		Guid id,
		CancellationToken cancellationToken)
	{
		//if (id is null)
		//	return NotFound();

		var productVm = await coreApiClient.GetProduct(id, cancellationToken);
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
		Guid id,
		CancellationToken cancellationToken)
	{
		//if (id is null) return NotFound();

		var productVm = await coreApiClient.GetProduct(id, cancellationToken);
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

	[HttpPost]
	public async Task<IActionResult> MakeArchived(
		Guid id,
		CancellationToken cancellationToken)
	{
		await coreApiClient.ArchiveProduct(id, cancellationToken);
		return RedirectToAction(nameof(Index));
	}

	[HttpPost]
	public async Task<IActionResult> MakeActive(
		Guid id,
		CancellationToken cancellationToken)
	{
		await coreApiClient.ActivateProduct(id, cancellationToken);
		return RedirectToAction(nameof(Index));
	}
}
