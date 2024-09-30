using DynamicPriceCore.MediatR.OrderEntity.Commands;
using DynamicPriceCore.MediatR.OrderEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;
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

	[HttpGet("/api/Orders/Cart/{customerId}/{companyId}")]
	public async Task<ActionResult<Order>> CartOrderDetails(int? customerId, int? companyId, CancellationToken cancellationToken)
	{
		var cartOrder = await _mediator.Send(new GetCartOrderQuery((int)customerId, (int)companyId), cancellationToken);

		return cartOrder == null 
			? NotFound(new { message = "Cart is empty." }) 
			: Ok(cartOrder);
	}

	[HttpGet("/api/Orders/Add/{customerId}/{productId}")]
	public async Task<ActionResult<Order>> AddProduct(int? customerId, int? productId)
	{
		var cartOrder = await _mediator.Send(new AddProductToOrderCommand(customerId.ToString(), productId.ToString()));
		return Ok(cartOrder);
	}

	
	[HttpGet("/api/Orders/Remove/{customerId}/{productId}")]
	public async Task<ActionResult<Order>> RemoveProduct(int? customerId, int? productId)
	{
		var cartOrder = await _mediator.Send(new RemoveProductFromOrderCommand(customerId.ToString(), productId.ToString()));
		return Ok(cartOrder);
	}

	[HttpGet("/api/Orders/Confirm/{customerId}/{cartOrderId}")]
	public async Task<ActionResult<int>> ConfirmOrder(int? customerId, int? cartOrderId, CancellationToken cancellationToken)
	{
		var receiveKey = await _mediator.Send(new ConfirmOrderCommand((int)customerId, (int)cartOrderId), cancellationToken);
		return Ok(receiveKey);
	}

	[HttpGet("/api/{userId}/CompanyOrders")]
	public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetCompanyOrders(string userId, CancellationToken cancellationToken)
	{
		var ordersVm = await _mediator.Send(new GetCompanyOrdersQuery(userId), cancellationToken);
		return Ok(ordersVm);
	}

	[HttpGet("/api/CompanyOrders/{orderId}")]
	public async Task<ActionResult<OrderViewModel>> GetCompanyOrder(string orderId)
	{
		var ordersVm = await _mediator.Send(new GetCompanyOrderDetailsQuery(orderId));
		return Ok(ordersVm);
	}

	[HttpGet("/api/CompanyOrders/FindByReceiveKey/{key}")]
	public async Task<ActionResult<int>> GetCompanyOrderByReceiveKey(string key)
	{
		var orderId = await _mediator.Send(new GetOrderIdByReceiveKeyQuery(key));
		return orderId;
	}

	[HttpGet("/api/CompanyOrders/Complete/{orderId}")]
	public async Task<ActionResult<int>> CompleteOrder(string orderId)
	{
		var id =  await _mediator.Send(new CompleteOrderCommand(orderId));
		return id;
	}

	[HttpGet("/api/{userId}/CompanyOrders/Statistics")]
	public async Task<ActionResult<OrderStatistics>> GetCompanyStatistics(string userId, CancellationToken cancellationToken)
	{
		var orderStatistics = await _mediator.Send(new GetCompanyStatisticsQuery(userId), cancellationToken);
		return Ok(orderStatistics);
	}
}
