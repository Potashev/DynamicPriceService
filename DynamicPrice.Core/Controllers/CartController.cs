using DynamicPrice.Core.MediatR.CartEntity.Commands;
using DynamicPrice.Core.MediatR.CartEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
public class CartController : ControllerBase
{
	private readonly IMediator _mediator;
	public CartController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet]
	public async Task<ActionResult<CartViewModel>> GetCartDetails([FromQuery(Name = "company-id")] string companyId, CancellationToken cancellationToken)
	{
		var cart = await _mediator.Send(new GetCartDetailsQuery(Convert.ToInt32(companyId)), cancellationToken);
		return Ok(cart);
	}

	[HttpPost("items")]
	public async Task<ActionResult<int>> AddProduct([FromBody] int? productId)
	{
		var cart = await _mediator.Send(new AddProductToCartCommand(productId.ToString()));
		return Ok(cart.Company.CompanyId);
	}

	[HttpDelete("items/{productId}")]
	public async Task<ActionResult<int>> RemoveProduct(int? productId)
	{
		var cart = await _mediator.Send(new RemoveProductFromCartCommand(productId.ToString()));
		return Ok(cart.Company.CompanyId);
	}
}
