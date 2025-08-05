using DynamicPriceCore.MediatR.CompanyEntity.Queries;
using DynamicPriceCore.MediatR.CustomerEntity.Commands;
using DynamicPriceCore.MediatR.CustomerEntity.Queries;
using DynamicPriceCore.MediatR.PriceRuleEntity.Commands;
using DynamicPriceCore.Models;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceCore.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
public class CustomersController : ControllerBase
{
	private readonly IMediator _mediator;
	public CustomersController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("me")]
	public async Task<ActionResult<CustomerInfoViewModel>> GetCustomerInfo(CancellationToken cancellationToken)
	{
		var customerInfo = await _mediator.Send(new GetCustomerInfoQuery(), cancellationToken);
		return Ok(customerInfo);
	}

	[HttpPut("me/balance")]
	public async Task<IActionResult> TopUp([FromBody] BalanceViewModel balanceVm)
	{
		await _mediator.Send(new TopUpBalanceCommand(balanceVm));
		return Ok();
	}

}
