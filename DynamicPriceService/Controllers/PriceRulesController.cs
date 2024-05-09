using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DynamicPriceService.Data;
using DynamicPriceService.Models;
using System.Net.Http.Headers;
using DynamicPriceService.Services;

namespace DynamicPriceService.Controllers;
public class PriceRulesController : Controller
{
	private readonly DynamicPriceServiceContext _context;
	private IActiveCompaniesService _activeCompaniesService;

	public PriceRulesController(DynamicPriceServiceContext context, IActiveCompaniesService activeCompaniesService)
	{
		_context = context;
		_activeCompaniesService = activeCompaniesService;
	}

	public async Task<IActionResult> Details()
	{
		var company = GetCompany();

		var priceRule = await _context.PriceRule
			.FirstOrDefaultAsync(m => m.PriceRuleId == company.CompanyId);

		if (priceRule == null)
		{
			priceRule = CreateDefaultRule(company);
		}

		ViewData["RuleStatus"] = GetRuleStatus(priceRule);

		return View(priceRule);
	}

	private Company GetCompany() => _context.Company.FirstOrDefault();

	private string GetRuleStatus(PriceRule rule) => _activeCompaniesService.IsActive(GetCompany()) ?
		"Running" :
		"Not running";

	private PriceRule CreateDefaultRule(Company company)
	{
		var priceRule = new PriceRule
		{
			Increase = 10,
			Reduction = 1,
			NoSellTime = new TimeSpan(0, 0, 10),
			Company = company
		};

		_context.Add(priceRule);
		_context.SaveChanges(); //todo: make async

		return priceRule;
	}

	// GET: priceRules/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null || _context.PriceRule == null)
		{
			return NotFound();
		}

		var priceRule = await _context.PriceRule.FindAsync(id);
		if (priceRule == null)
		{
			return NotFound();
		}
		return View(priceRule);
	}
	public async Task<IActionResult> Run()
	{
		_activeCompaniesService.Add(GetCompany());

		return RedirectToAction(nameof(Details));
	}

	// POST: priceRules/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, /*[Bind("Id,Increase,Reduction,Downtime")]*/ PriceRule priceRule)
	{
		if (id != priceRule.PriceRuleId)
		{
			return NotFound();
		}

		//if (ModelState.IsValid)
		//{
		try
		{
			_context.Update(priceRule);
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!PriceRuleExists(priceRule.PriceRuleId))
			{
				return NotFound();
			}
			else
			{
				throw;
			}
		}
		return RedirectToAction(nameof(Details));
		//}
		//return View(priceRule);
	}

	private bool PriceRuleExists(int id)
	{
		return (_context.PriceRule?.Any(e => e.PriceRuleId == id)).GetValueOrDefault();
	}
}
