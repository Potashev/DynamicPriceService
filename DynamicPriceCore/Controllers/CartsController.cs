using DynamicPriceCore.MediatR.CartEntity.Commands;
using DynamicPriceCore.MediatR.CartEntity.Queries;
using DynamicPriceCore.MediatR.OrderEntity.Commands;
using DynamicPriceCore.MediatR.OrderEntity.Queries;
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
public class CartsController : ControllerBase
{
	private readonly IMediator _mediator;
	public CartsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("{companyId}")]
	public async Task<ActionResult<CartViewModel>> GetCartDetails(int? companyId, CancellationToken cancellationToken)
	{
		var cart = await _mediator.Send(new GetCartDetailsQuery((int)companyId), cancellationToken);

		return cart == null
			? NotFound(new { message = "Cart is empty." })
			: Ok(cart);
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
