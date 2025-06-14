using DynamicPriceCore.MediatR.OrderEntity.Commands;
using DynamicPriceCore.MediatR.OrderEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

	[HttpGet("/api/Orders/Cart/{companyId}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<Order>> CartOrderDetails(int? companyId, CancellationToken cancellationToken)
	{
		var cartOrder = await _mediator.Send(new GetCartOrderQuery((int)companyId), cancellationToken);

		return cartOrder == null 
			? NotFound(new { message = "Cart is empty." }) 
			: Ok(cartOrder);
	}

	[HttpGet("/api/Orders/Add/{productId}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<Order>> AddProduct(int? productId)
	{
		var cartOrder = await _mediator.Send(new AddProductToOrderCommand(productId.ToString()));
		return Ok(cartOrder);
	}

	
	[HttpGet("/api/Orders/Remove/{productId}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<Order>> RemoveProduct(int? productId)
	{
		var cartOrder = await _mediator.Send(new RemoveProductFromOrderCommand(productId.ToString()));
		return Ok(cartOrder);
	}

	[HttpGet("/api/Orders/Confirm/{cartOrderId}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<int>> ConfirmOrder(int? cartOrderId, CancellationToken cancellationToken)
	{
		var receiveKey = await _mediator.Send(new ConfirmOrderCommand((int)cartOrderId), cancellationToken);
		return Ok(receiveKey);
	}

	[HttpGet("/api/CompanyOrders")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetCompanyOrders(CancellationToken cancellationToken)
	{
		//update
		var ordersVm = await _mediator.Send(new GetCompanyOrdersQuery(), cancellationToken);
		return Ok(ordersVm);
	}

	[HttpGet("/api/CompanyOrders/{orderId}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<OrderViewModel>> GetCompanyOrder(string orderId)
	{
		var ordersVm = await _mediator.Send(new GetCompanyOrderDetailsQuery(orderId));
		return Ok(ordersVm);
	}

	[HttpGet("/api/CompanyOrders/FindByReceiveKey/{key}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<int>> GetCompanyOrderByReceiveKey(string key)
	{
		var orderId = await _mediator.Send(new GetOrderIdByReceiveKeyQuery(key));
		return orderId;
	}

	[HttpGet("/api/CompanyOrders/{orderId}/Complete")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<int>> CompleteOrder(string orderId)
	{
		var id =  await _mediator.Send(new CompleteOrderCommand(orderId));
		return id;
	}

	[HttpGet("/api/CompanyOrders/Statistics")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<OrderStatistics>> GetCompanyStatistics(CancellationToken cancellationToken)
	{
		var orderStatistics = await _mediator.Send(new GetCompanyStatisticsQuery(), cancellationToken);
		return Ok(orderStatistics);
	}
}
