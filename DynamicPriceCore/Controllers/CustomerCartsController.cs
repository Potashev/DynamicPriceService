using DynamicPriceCore.MediatR.CartEntity.Commands;
using DynamicPriceCore.MediatR.CartEntity.Queries;
using DynamicPriceCore.MediatR.OrderEntity.Commands;
using DynamicPriceCore.MediatR.OrderEntity.Queries;
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
public class CustomerCartsController : ControllerBase
{
	private readonly IMediator _mediator;
	public CustomerCartsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("/api/Orders/Cart/{companyId}")]
	public async Task<ActionResult<Cart>> GetCartDetails(int? companyId, CancellationToken cancellationToken)
	{
		var cart = await _mediator.Send(new GetCartDetailsQuery((int)companyId), cancellationToken);

		return cart == null
			? NotFound(new { message = "Cart is empty." })
			: Ok(cart);
	}

	[HttpGet("/api/Orders/Add/{productId}")]
	public async Task<ActionResult<Order>> AddProduct(int? productId)
	{
		var cartOrder = await _mediator.Send(new AddProductToCartCommand(productId.ToString()));
		return Ok(cartOrder);
	}


	[HttpGet("/api/Orders/Remove/{productId}")]
	public async Task<ActionResult<Order>> RemoveProduct(int? productId)
	{
		var cartOrder = await _mediator.Send(new RemoveProductFromCartCommand(productId.ToString()));
		return Ok(cartOrder);
	}

	[HttpGet("/api/Orders/Confirm/{cartOrderId}")]
	public async Task<ActionResult<int>> ConfirmOrder(int? cartOrderId, CancellationToken cancellationToken)
	{
		var receiveKey = await _mediator.Send(new ConfirmCartCommand((int)cartOrderId), cancellationToken);
		return Ok(receiveKey);
	}
}
