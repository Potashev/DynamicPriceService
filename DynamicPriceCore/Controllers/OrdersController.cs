using DynamicPriceCore.MediatR.OrderEntity.Commands;
using DynamicPriceCore.MediatR.OrderEntity.Queries;
using DynamicPriceCore.Models;
using DynamicPriceCore.ViewModels;
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

	[HttpPost]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<int>> ConfirmOrder([FromBody]int? cartOrderId, CancellationToken cancellationToken)
	{
		var receiveKey = await _mediator.Send(new ConfirmOrderCommand((int)cartOrderId), cancellationToken);
		return Ok(receiveKey);
	}

	[HttpGet]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetCompanyOrders(CancellationToken cancellationToken)
	{
		var ordersVm = await _mediator.Send(new GetCompanyOrdersQuery(), cancellationToken);
		return Ok(ordersVm);
	}

	[HttpGet("{orderId}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<OrderViewModel>> GetCompanyOrder(string orderId)
	{
		var orderVm = await _mediator.Send(new GetCompanyOrderDetailsQuery(orderId));
		return Ok(orderVm);
	}

	[HttpGet("by-receive-key/{key}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<int>> GetOrderByReceiveKey(string key)
	{
		var orderId = await _mediator.Send(new GetOrderIdByReceiveKeyQuery(key));
		return orderId;
	}

	[HttpPatch("{orderId}/complete")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<int>> CompleteOrder(string orderId)
	{
		var id =  await _mediator.Send(new CompleteOrderCommand(orderId));
		return id;
	}

	[HttpGet("statistics")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<OrdersStatistics>> GetCompanyStatistics(CancellationToken cancellationToken)
	{
		var orderStatistics = await _mediator.Send(new GetCompanyStatisticsQuery(), cancellationToken);
		return Ok(orderStatistics);
	}
}
