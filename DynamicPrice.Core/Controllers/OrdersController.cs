using DynamicPrice.Core.MediatR.OrderEntity.Commands;
using DynamicPrice.Core.MediatR.OrderEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.Controllers;

[ApiController]
public class OrdersController : ControllerBase
{
	private readonly IMediator _mediator;
	public OrdersController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost("api/customer/order/confirm")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<int>> ConfirmOrder([FromBody] int? cartId, CancellationToken cancellationToken)
	{
		var orderId = await _mediator.Send(new ConfirmOrderCommand((int)cartId), cancellationToken);
		return Ok(orderId);
	}

	[HttpPatch("api/customer/order/cancel")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<int>> CancelOrder([FromBody] int? orderId, CancellationToken cancellationToken)
	{
		await _mediator.Send(new CancelOrderCommand((int)orderId), cancellationToken);
		return Ok(orderId);
	}

	[HttpGet("api/customer/order/{orderId}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<OrderInfoViewModel>> GetCustomerOrder(string orderId)
	{
		var orderVm = await _mediator.Send(new GetCustomerOrderDetailsQuery(orderId));
		return Ok(orderVm);
	}

	[HttpGet("api/company/orders")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetCompanyOrders(CancellationToken cancellationToken)
	{
		var ordersVm = await _mediator.Send(new GetCompanyOrdersQuery(), cancellationToken);
		return Ok(ordersVm);
	}

	[HttpGet("api/company/orders/{orderId}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<OrderViewModel>> GetCompanyOrder(string orderId)
	{
		var orderVm = await _mediator.Send(new GetCompanyOrderDetailsQuery(orderId));
		return Ok(orderVm);
	}

	[HttpGet("api/company/orders/by-receive-key/{key}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<int>> GetOrderByReceiveKey(string key)
	{
		var orderId = await _mediator.Send(new GetOrderIdByReceiveKeyQuery(key));
		return orderId;
	}

	[HttpPatch("api/company/orders/{orderId}/ready")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult> ReadyForReceive(string orderId)
	{
		await _mediator.Send(new ReadyForReceiveOrderCommand(orderId));
		return Ok();
	}

	[HttpPatch("api/company/orders/{orderId}/complete")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<int>> CompleteOrder(string orderId)
	{
		var id = await _mediator.Send(new CompleteOrderCommand(orderId));
		return id;
	}

	[HttpGet("api/company/orders/statistics")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<OrdersStatistics>> GetCompanyStatistics(CancellationToken cancellationToken)
	{
		var orderStatistics = await _mediator.Send(new GetCompanyStatisticsQuery(), cancellationToken);
		return Ok(orderStatistics);
	}
}
