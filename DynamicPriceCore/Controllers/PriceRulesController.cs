using DynamicPriceCore.MediatR.PriceRuleEntity.Commands;
using DynamicPriceCore.MediatR.PriceRuleEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using MediatR;
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

	[Route("/api/{userId}/PriceRule/Details")]
	public async Task<ActionResult<PriceRuleWithStatus>> GetPriceRule(string userId)
	{
		return await _mediator.Send(new GetPriceRuleWithStatusQuery(userId));
	}

	// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
	[Route("/api/{userId}/PriceRule/Edit")]
	public async Task<IActionResult> PutProduct(int userId, PriceRuleViewModel priceRuleVm)
	{
		var priceRuleId = await _mediator.Send(new EditPriceRuleCommand(priceRuleVm));
		return Ok(priceRuleId);
	}

	[Route("/api/{userId}/PriceRule/Run")]
	[HttpGet]
	public async Task<ActionResult> RunPriceReducing(string userId)
	{
		await _mediator.Send(new RunPriceReducingCommand(userId));
		return Ok();
	}
}