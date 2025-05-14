using DynamicPriceCore.MediatR.CompanyEntity.Queries;
using DynamicPriceCore.MediatR.CustomerEntity.Queries;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceCore.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
public class CustomerController : ControllerBase
{
	private readonly IMediator _mediator;
	public CustomerController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("/api/CustomerInfo")]
	public async Task<ActionResult<IEnumerable<Company>>> GetCustomerInfo(CancellationToken cancellationToken)
	{
		var customerInfo = await _mediator.Send(new GetCustomerInfoQuery(), cancellationToken);
		return Ok(customerInfo);
	}
}
