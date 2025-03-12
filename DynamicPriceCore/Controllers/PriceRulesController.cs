using DynamicPriceCore.MediatR.PriceRuleEntity.Commands;
using DynamicPriceCore.MediatR.PriceRuleEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceCore.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PriceRulesController : ControllerBase
{
	private readonly IMediator _mediator;

	private readonly UserManager<IdentityUser> _userManager;

	public PriceRulesController(IMediator mediator, UserManager<IdentityUser> userManager)
	{
		_mediator = mediator;
		_userManager = userManager;
	}

	[HttpGet("/api/{userId}/PriceRule/Details")]
	[Authorize]
	public async Task<ActionResult<PriceRuleWithStatus>> Get(string userId)
	{
		var manager = new Manager
		{
			UserName = "Alex Avtozapchasti",
			Email = "alex@mail.ru",
			CompanyId = 1,
		};
		var result = await _userManager.CreateAsync(manager, "123");
		if (result.Succeeded)
		{
			await _userManager.AddToRoleAsync(manager, "Manager");
		}

		var customer = new Customer
		{
			UserName = "cust1",
			Email = "testcust@mail.ru",
			Balance = 10m,
		};

		var result2 = await _userManager.CreateAsync(customer, "model.Password");

		if (result2.Succeeded)
		{
			await _userManager.AddToRoleAsync(customer, "Customer");
			//return Ok();
		}



		return await _mediator.Send(new GetPriceRuleWithStatusQuery(userId));
	}

	// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
	[HttpPost("/api/PriceRule/Edit")]
	public async Task<IActionResult> Edit(PriceRuleViewModel priceRuleVm)
	{
		var priceRuleId = await _mediator.Send(new EditPriceRuleCommand(priceRuleVm));
		return Ok(priceRuleId);
	}

	[HttpGet("/api/{userId}/PriceRule/Run")]
	public async Task<ActionResult> RunPriceReducing(string userId)
	{
		await _mediator.Send(new PriceReducingCommand(userId, true));
		return Ok();
	}

	[HttpGet("/api/{userId}/PriceRule/Stop")]
	public async Task<ActionResult> StopPriceReducing(string userId)
	{
		await _mediator.Send(new PriceReducingCommand(userId, false));
		return Ok();
	}
}