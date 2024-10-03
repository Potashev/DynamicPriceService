using DynamicPriceCore.MediatR.PriceRuleEntity.Commands;
using DynamicPriceCore.MediatR.PriceRuleEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceCore.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PriceRulesController : ControllerBase
{
	private readonly IMediator _mediator;

	public PriceRulesController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("/api/{userId}/PriceRule/Details")]
	[Authorize]
	public async Task<ActionResult<PriceRuleWithStatus>> Get(string userId)
	{
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