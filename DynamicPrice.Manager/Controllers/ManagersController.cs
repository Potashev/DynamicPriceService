using DynamicPrice.Manager.ApiClients;
using DynamicPrice.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Manager.Controllers;

public class ManagersController : BaseController
{
	public ManagersController(ICoreApiClient coreApiClient)
	: base(coreApiClient) { }

	public async Task<IActionResult> Index(CancellationToken cancellationToken)
		=> View(await CoreApiClient.GetCompanyManagers(cancellationToken));

	public IActionResult RegisterManager()
		=> View();

	[HttpPost]
	[ValidateAntiForgeryToken]
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
