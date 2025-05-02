using DynamicPriceCore.MediatR.PriceRuleEntity.Commands;
using DynamicPriceCore.MediatR.PriceRuleEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceCore.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
public class PriceRulesController : ControllerBase
{
	private readonly IMediator _mediator;

	public PriceRulesController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("/api/PriceRule")]
	public async Task<ActionResult<PriceRuleWithStatus>> Get()
	{
		return await _mediator.Send(new GetPriceRuleWithStatusQuery());
	}

	// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
	[HttpPut("/api/PriceRule")]
	public async Task<IActionResult> Edit(PriceRuleViewModel priceRuleVm)
	{
		var priceRuleId = await _mediator.Send(new EditPriceRuleCommand(priceRuleVm));
		return Ok(priceRuleId);
	}

	[HttpGet("/api/PriceRule/Run")]
	public async Task<ActionResult> RunPriceReducing()
	{
		await _mediator.Send(new PriceReducingCommand(true));
		return Ok();
	}

	[HttpGet("/api/PriceRule/Stop")]
	public async Task<ActionResult> StopPriceReducing()
	{
		await _mediator.Send(new PriceReducingCommand(false));
		return Ok();
	}
}