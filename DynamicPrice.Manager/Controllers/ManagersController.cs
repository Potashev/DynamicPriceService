using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class ManagersController(
	ICoreApiClient coreApiClient) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await coreApiClient.GetCompanyManagers(cancellationToken));

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

		await coreApiClient.RegisterManager(registerVm, cancellationToken);

		return RedirectToAction(nameof(Index));
	}
}
