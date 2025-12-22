using DynamicPrice.Core.MediatR.PriceRuleEntity.Commands;
using DynamicPrice.Core.MediatR.PriceRuleEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.Controllers;

[Route("api/company/price-rule")]
[ApiController]
[Authorize("ManagerPolicy")]
public class PriceRuleController : ControllerBase
{
	private readonly IMediator _mediator;

	public PriceRuleController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet]
	public async Task<ActionResult<PriceRuleWithStatus>> Get()
	{
		return await _mediator.Send(new GetPriceRuleWithStatusQuery());
	}

	[HttpPut]
	public async Task<IActionResult> Edit(PriceRuleViewModel priceRuleVm)
	{
		var priceRuleId = await _mediator.Send(new EditPriceRuleCommand(priceRuleVm));
		return Ok(priceRuleId);
	}

	[HttpPost("run")]
	public async Task<ActionResult> RunPriceReducing()
	{
		await _mediator.Send(new PriceReducingCommand(true));
		return Ok();
	}

	[HttpPost("stop")]
	public async Task<ActionResult> StopPriceReducing()
	{
		await _mediator.Send(new PriceReducingCommand(false));
		return Ok();
	}
}