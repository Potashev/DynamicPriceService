using Microsoft.AspNetCore.Mvc;
using DynamicPriceService.Services;
using MediatR;
using DynamicPriceService.MediatR.PriceRuleEntity.Queries;
using DynamicPriceService.MediatR.ViewModel;
using DynamicPriceService.MediatR.PriceRuleEntity.Commands;

namespace DynamicPriceService.Controllers;
public class PriceRulesController : Controller
{
    private readonly IMediator _mediator;
    private IActiveCompaniesService _activeCompaniesService;

    public PriceRulesController(IMediator mediator, IActiveCompaniesService activeCompaniesService)
    {
        _mediator = mediator;
        _activeCompaniesService = activeCompaniesService;
    }

    public async Task<IActionResult> Details()
    {
        //todo: pass to query some company identificator
        var priceRuleVm = await _mediator.Send(new GetPriceRuleDetailsQuery());

        // use mediator also?
        //ViewData["RuleStatus"] = GetRuleStatus();

        return View(priceRuleVm);
    }

    //private string GetRuleStatus() => _activeCompaniesService.IsActive(GetCompany()) ?
    //    "Running" :
    //    "Not running";

    // GET: priceRules/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        var priceRuleVm = await _mediator.Send(new GetPriceRuleDetailsQuery());
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
        //_activeCompaniesService.Add(GetCompany());
        return RedirectToAction(nameof(Details));
    }
}
