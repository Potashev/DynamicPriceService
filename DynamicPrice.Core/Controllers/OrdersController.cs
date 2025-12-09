using DynamicPrice.Core.MediatR.OrderEntity.Commands;
using DynamicPrice.Core.MediatR.OrderEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.Controllers;

//todo: separate CompanyOrders and CustomerOrder controllers?
/// <summary>
/// Контроллер для управления заказами как со стороны клиента, так и со стороны менеджера компании.
/// Методы разделены по политикам аутентификации: CustomerPolicy и ManagerPolicy.
/// </summary>
[ApiController]
public class OrdersController : ControllerBase
{
	private readonly IMediator _mediator;
	public OrdersController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Подтвердить корзину и создать заказ на её основе.
	/// </summary>
	/// <param name="cartId">Идентификатор корзины, передаётся в теле запроса.</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Идентификатор созданного заказа.</returns>
	[HttpPost("api/customer/order/confirm")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<int>> ConfirmOrder([FromBody] int? cartId, CancellationToken cancellationToken)
	{
		var orderId = await _mediator.Send(new ConfirmOrderCommand((int)cartId), cancellationToken);
		return Ok(orderId);
	}

	/// <summary>
	/// Отменить заказ клиента.
	/// </summary>
	/// <param name="orderId">Идентификатор заказа для отмены (в теле запроса).</param>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>200 OK при успешной отмене.</returns>
	[HttpPatch("api/customer/order/cancel")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<int>> CancelOrder([FromBody] int? orderId, CancellationToken cancellationToken)
	{
		await _mediator.Send(new CancelOrderCommand((int)orderId), cancellationToken);
		return Ok(orderId);
	}

	/// <summary>
	/// Получить детали заказа клиента по его идентификатору.
	/// </summary>
	/// <param name="orderId">Идентификатор заказа.</param>
	/// <returns>Информация о заказе <see cref="OrderInfoViewModel"/>.</returns>
	[HttpGet("api/customer/order/{orderId}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public async Task<ActionResult<OrderInfoViewModel>> GetCustomerOrder(string orderId)
	{
		var orderVm = await _mediator.Send(new GetCustomerOrderDetailsQuery(orderId));
		return Ok(orderVm);
	}

	/// <summary>
	/// Получить список заказов компании.
	/// </summary>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Список заказов <see cref="OrderViewModel"/>.</returns>
	[HttpGet("api/company/orders")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetCompanyOrders(CancellationToken cancellationToken)
	{
		var ordersVm = await _mediator.Send(new GetCompanyOrdersQuery(), cancellationToken);
		return Ok(ordersVm);
	}

	/// <summary>
	/// Получить детали заказа компании по идентификатору.
	/// </summary>
	/// <param name="orderId">Идентификатор заказа.</param>
	/// <returns>Детали заказа <see cref="OrderViewModel"/>.</returns>
	[HttpGet("api/company/orders/{orderId}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<OrderViewModel>> GetCompanyOrder(string orderId)
	{
		var orderVm = await _mediator.Send(new GetCompanyOrderDetailsQuery(orderId));
		return Ok(orderVm);
	}

	/// <summary>
	/// Найти идентификатор заказа по ключу получения (receive key).
	/// Используется на точке выдачи для быстрого поиска заказа.
	/// </summary>
	/// <param name="key">Ключ получения заказа.</param>
	/// <returns>Идентификатор заказа (int).</returns>
	[HttpGet("api/company/orders/by-receive-key/{key}")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<int>> GetOrderByReceiveKey(string key)
	{
		var orderId = await _mediator.Send(new GetOrderIdByReceiveKeyQuery(key));
		return orderId;
	}

	/// <summary>
	/// Пометить заказ как готовый к выдаче.
	/// </summary>
	/// <param name="orderId">Идентификатор заказа.</param>
	/// <returns>200 OK при успешной операции.</returns>
	[HttpPatch("api/company/orders/{orderId}/ready")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult> ReadyForReceive(string orderId)
	{
		await _mediator.Send(new ReadyForReceiveOrderCommand(orderId));
		return Ok();
	}

	/// <summary>
	/// Пометить заказ как завершённый (выдан).
	/// </summary>
	/// <param name="orderId">Идентификатор заказа.</param>
	/// <returns>Идентификатор заказа после завершения.</returns>
	[HttpPatch("api/company/orders/{orderId}/complete")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<int>> CompleteOrder(string orderId)
	{
		var id = await _mediator.Send(new CompleteOrderCommand(orderId));
		return id;
	}

	/// <summary>
	/// Получить статистику по заказам компании.
	/// </summary>
	/// <param name="cancellationToken">Токен отмены операции.</param>
	/// <returns>Статистика заказов <see cref="OrdersStatistics"/>.</returns>
	[HttpGet("api/company/orders/statistics")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
	public async Task<ActionResult<OrdersStatistics>> GetCompanyStatistics(CancellationToken cancellationToken)
	{
		var orderStatistics = await _mediator.Send(new GetCompanyStatisticsQuery(), cancellationToken);
		return Ok(orderStatistics);
	}
}
