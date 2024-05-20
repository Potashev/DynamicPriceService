using Microsoft.AspNetCore.Mvc;
using DynamicPriceService.Services;
using MediatR;
using DynamicPriceService.MediatR.PriceRuleEntity.Queries;
using DynamicPriceService.MediatR.ViewModel;
using DynamicPriceService.MediatR.PriceRuleEntity.Commands;
using DynamicPriceService.Data;
using DynamicPriceService.Models;

namespace DynamicPriceService.Controllers;
public class PriceRulesController : Controller
{
	private readonly IMediator _mediator;
	//todo: temp field to pass in mediator - remove later
	private readonly DynamicPriceServiceContext _context;

	public PriceRulesController(IMediator mediator, DynamicPriceServiceContext context)
	{
		_mediator = mediator;
		_context = context;
	}

	public async Task<IActionResult> Details()
	{
		var priceRuleVm = await _mediator.Send(new GetPriceRuleDetailsQuery(GetCompany()));

		ViewData["RuleStatus"] = await _mediator.Send(new GetPriceRuleStatusQuery(GetCompany())) ?
			"Running" :
			"Not running";

		return View(priceRuleVm);

	}

	// GET: priceRules/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		var priceRuleVm = await _mediator.Send(new GetPriceRuleDetailsQuery(GetCompany()));
		return View(priceRuleVm);
	}

	// POST: priceRules/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, [Bind("PriceRuleId,Increase,Reduction,NoSellTime")] PriceRuleViewModel priceRuleVm)
	{
		if (id != priceRuleVm.PriceRuleId)
		{
			return NotFound();
		}

		if (ModelState.IsValid)
		{
			await _mediator.Send(new EditPriceRuleCommand(priceRuleVm));
			return RedirectToAction(nameof(Details));
		}
		return View(priceRuleVm);
	}

	public async Task<IActionResult> Run()
	{
		await _mediator.Send(new RunPriceReducingCommand(GetCompany()));
		return RedirectToAction(nameof(Details));
	}

	private Company GetCompany() => _context.Company.FirstOrDefault();
}
