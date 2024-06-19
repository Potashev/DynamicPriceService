using Microsoft.AspNetCore.Mvc;
using MediatR;
using DynamicPriceService.MediatR.PriceRuleEntity.Queries;
using DynamicPriceService.MediatR.PriceRuleEntity.Commands;
using DynamicPriceService.Data;
using DynamicPriceService.Models;

namespace DynamicPriceService.Controllers;
public class PriceRulesController : Controller
{
	private readonly IMediator _mediator;
    //todo: temp field to pass in mediator - remove later
    private readonly string _userId = "1";

    public PriceRulesController(IMediator mediator, DynamicPriceServiceContext context)
        => _mediator = mediator;

	public async Task<IActionResult> Details()
	{
		var priceRuleWithStatus = await _mediator.Send(new GetPriceRuleWithStatusQuery(_userId));

		ViewData["RuleStatus"] = priceRuleWithStatus.IsActive ?
			"Running" :
			"Not running";

		return View(priceRuleWithStatus.PriceRule);

	}

	// GET: priceRules/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		var priceRuleVm = await _mediator.Send(new GetPriceRuleDetailsQuery(_userId));
		return View(priceRuleVm);
	}

	// POST: priceRules/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, EditPriceRuleCommand command)
	{
        //if (id != priceRule.PriceRuleId)
        //{
        //	return NotFound();
        //}

        //      ModelState.Remove("Company");
        //if (ModelState.IsValid)
        //{
        //          await _mediator.Send(new EditPriceRuleCommand(priceRule));
        //          return RedirectToAction(nameof(Details));
        //}
        //return View(priceRule);

        var priceRule = await _mediator.Send(command);
        return Ok(priceRule);
	}

	public async Task<IActionResult> Run()
	{
		await _mediator.Send(new RunPriceReducingCommand(_userId));
		return RedirectToAction(nameof(Details));
	}
}
