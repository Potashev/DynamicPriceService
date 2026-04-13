using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class ManagersController(ICoreApiClient CoreApiClient) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await CoreApiClient.GetCompanyManagers(cancellationToken));

	[HttpGet]
	public IActionResult RegisterManager()
		=> View();

	[HttpPost]
	public async Task<IActionResult> RegisterManager(
		RegisterRequest registerVm,
		CancellationToken cancellationToken)
	{
		if (!ModelState.IsValid)
			return View(registerVm);

		await CoreApiClient.RegisterManager(registerVm, cancellationToken);

		return RedirectToAction(nameof(Index));
	}
}
