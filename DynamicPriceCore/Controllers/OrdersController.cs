using DynamicPriceCore.MediatR.OrderEntity.Commands;
using DynamicPriceCore.MediatR.OrderEntity.Queries;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
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
	[Route("/api/Orders/Add/{customerId}/{productId}")]
	public async Task<ActionResult<Order>> AddProduct(int? customerId, int? productId)
	{
		var cartOrder = await _mediator.Send(new AddProductToOrderCommand(customerId.ToString(), productId.ToString()));
		return Ok(cartOrder);
	}

	
	[HttpGet("/api/Orders/Remove/{customerId}/{productId}")]
	public async Task<ActionResult<Order>> RemoveProduct(int? customerId, int? productId)
	{
		await _mediator.Send(new RemoveProductFromOrderCommand(customerId.ToString(), productId.ToString()));
		return Ok();
	}

	[HttpGet]
	[Route("/api/Orders/Confirm/{customerId}/{cartOrderId}")]
	public async Task<ActionResult<int>> ConfirmOrder(int? customerId, int? cartOrderId)
	{
		var receiveKey = await _mediator.Send(new ConfirmOrderCommand((int)customerId, (int)cartOrderId));
		return Ok(receiveKey);
	}

	[HttpGet("/api/{userId}/CompanyOrders")]
	public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetCompanyOrders(string userId)
	{
		var ordersVm = await _mediator.Send(new GetCompanyOrdersQuery(userId));
		return Ok(ordersVm);
	}

	[HttpGet("/api/CompanyOrders/{orderId}")]
	public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetCompanyOrder(string orderId)
	{
		var ordersVm = await _mediator.Send(new GetCompanyOrderDetailsQuery(orderId));
		return Ok(ordersVm);
	}

	[HttpGet("/api/{userId}/CompanyOrders/Statistics")]
	public async Task<ActionResult<OrderStatistics>> GetCompanyStatistics(string userId)
	{
		var orderStatistics = await _mediator.Send(new GetCompanyStatisticsQuery(userId));
		return Ok(orderStatistics);
	}
}
