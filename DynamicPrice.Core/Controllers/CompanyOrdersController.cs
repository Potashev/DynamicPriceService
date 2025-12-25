//using DynamicPrice.Core.MediatR.OrderEntity.Commands;
//using DynamicPrice.Core.MediatR.OrderEntity.Queries;
//using DynamicPrice.Shared.Contracts.ViewModels;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace DynamicPrice.Core.Controllers;

//[ApiController]
//[Route("api/company/orders")]
//[Authorize(Roles = "Manager")]
//public class CompanyOrdersController : ControllerBase
//{
//	private readonly IMediator _mediator;
//	public CompanyOrdersController(IMediator mediator)
//	{
//		_mediator = mediator;
//	}

//	[HttpGet]
//	public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetCompanyOrders(CancellationToken cancellationToken)
//	{
//		var ordersVm = await _mediator.Send(new GetCompanyOrdersQuery(), cancellationToken);
//		return Ok(ordersVm);
//	}

//	[HttpGet("{orderId}")]
//	public async Task<ActionResult<OrderViewModel>> GetCompanyOrder(string orderId)
//	{
//		var orderVm = await _mediator.Send(new GetCompanyOrderDetailsQuery(orderId));
//		return Ok(orderVm);
//	}

//	[HttpGet("by-receive-key/{key}")]
//	public async Task<ActionResult<int>> GetOrderByReceiveKey(string key)
//	{
//		var orderId = await _mediator.Send(new GetOrderIdByReceiveKeyQuery(key));
//		return orderId;
//	}

//	[HttpPatch("{orderId}/ready")]
//	public async Task<ActionResult> ReadyForReceive(string orderId)
//	{
//		await _mediator.Send(new ReadyForReceiveOrderCommand(orderId));
//		return Ok();
//	}

//	[HttpPatch("{orderId}/complete")]
//	public async Task<ActionResult<int>> CompleteOrder(string orderId)
//	{
//		var id = await _mediator.Send(new CompleteOrderCommand(orderId));
//		return id;
//	}

//	[HttpGet("statistics")]
//	public async Task<ActionResult<OrdersStatistics>> GetCompanyStatistics(CancellationToken cancellationToken)
//	{
//		var orderStatistics = await _mediator.Send(new GetCompanyStatisticsQuery(), cancellationToken);
//		return Ok(orderStatistics);
//	}
//}
