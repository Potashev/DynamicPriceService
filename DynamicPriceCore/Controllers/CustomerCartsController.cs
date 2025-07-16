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
	public async Task<ActionResult<Order>> CartOrderDetails(int? companyId, CancellationToken cancellationToken)
	{
		var cartOrder = await _mediator.Send(new GetCartOrderQuery((int)companyId), cancellationToken);

		return cartOrder == null
			? NotFound(new { message = "Cart is empty." })
			: Ok(cartOrder);
	}

	[HttpGet("/api/Orders/Add/{productId}")]
	public async Task<ActionResult<Order>> AddProduct(int? productId)
	{
		var cartOrder = await _mediator.Send(new AddProductToOrderCommand(productId.ToString()));
		return Ok(cartOrder);
	}


	[HttpGet("/api/Orders/Remove/{productId}")]
	public async Task<ActionResult<Order>> RemoveProduct(int? productId)
	{
		var cartOrder = await _mediator.Send(new RemoveProductFromOrderCommand(productId.ToString()));
		return Ok(cartOrder);
	}

	[HttpGet("/api/Orders/Confirm/{cartOrderId}")]
	public async Task<ActionResult<int>> ConfirmOrder(int? cartOrderId, CancellationToken cancellationToken)
	{
		var receiveKey = await _mediator.Send(new ConfirmOrderCommand((int)cartOrderId), cancellationToken);
		return Ok(receiveKey);
	}
}
