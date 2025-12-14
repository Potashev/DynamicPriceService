using DynamicPrice.Core.MediatR.OrderEntity.Commands;
using DynamicPrice.Core.MediatR.OrderEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.Controllers
{
	[Route("api/customer/order")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
	public class CustomerOrderController : ControllerBase
	{
		private readonly IMediator _mediator;
		public CustomerOrderController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpPost("confirm")]
		public async Task<ActionResult<int>> ConfirmOrder([FromBody] int? cartId, CancellationToken cancellationToken)
		{
			var orderId = await _mediator.Send(new ConfirmOrderCommand((int)cartId), cancellationToken);
			return Ok(orderId);
		}

		[HttpPatch("cancel")]
		public async Task<ActionResult<int>> CancelOrder([FromBody] int? orderId, CancellationToken cancellationToken)
		{
			await _mediator.Send(new CancelOrderCommand((int)orderId), cancellationToken);
			return Ok(orderId);
		}

		[HttpGet("{orderId}")] //todo: use query param? (check cart)
		public async Task<ActionResult<OrderInfoViewModel>> GetCustomerOrder(string orderId)
		{
			var orderVm = await _mediator.Send(new GetCustomerOrderDetailsQuery(orderId));
			return Ok(orderVm);
		}
	}
}
