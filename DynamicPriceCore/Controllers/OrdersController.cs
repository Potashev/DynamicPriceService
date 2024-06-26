using DynamicPriceCore.MediatR.OrderEntity.Commands;
using DynamicPriceCore.MediatR.OrderEntity.Queries;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceCore.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
	private readonly IMediator _mediator;
	public OrdersController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet]
	[Route("/api/Orders/Cart/{customerId}/")]
	public async Task<ActionResult<Order>> CartOrderDetails(int? customerId)
	{
		var cartOrder = await _mediator.Send(new GetCartOrderQuery((int)customerId));
		return Ok(cartOrder);
	}

	[HttpGet]
	[Route("/api/Orders/{customerId}/{productId}")]
	public async Task<ActionResult<Order>> AddProduct(int? customerId, int? productId)
	{
		var cartOrder = await _mediator.Send(new AddProductToOrderCommand(customerId.ToString(), productId.ToString()));
		return Ok(cartOrder);
	}
}
