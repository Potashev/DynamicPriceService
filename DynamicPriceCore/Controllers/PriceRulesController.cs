using DynamicPriceCore.MediatR.PriceRuleEntity.Commands;
using DynamicPriceCore.MediatR.PriceRuleEntity.Queries;
using DynamicPriceCore.MediatR.ProductEntity.Commands;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
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
    //todo: temp field to pass in mediator - remove later
    private readonly string _userId = "1";

    public PriceRulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/PriceRules/5
    [HttpGet("{userId}")]
    public async Task<ActionResult<PriceRuleWithStatus>> GetPriceRule(string userId)
    {
        return await _mediator.Send(new GetPriceRuleWithStatusQuery(userId));
    }

    // PUT: api/PriceRules/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{userId}")]
    public async Task<IActionResult> PutProduct(int userId, PriceRuleViewModel priceRuleVm)
    {
        var priceRuleId = await _mediator.Send(new EditPriceRuleCommand(priceRuleVm));
        return Ok(priceRuleId);
    }

}
