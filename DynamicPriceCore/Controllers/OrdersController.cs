using DynamicPriceCore.MediatR.OrderEntity.Commands;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
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
	[Route("/api/Orders/{customerId}/{productId}")]
	public async Task<ActionResult> AddProduct(int? customerId, int? productId)
	{
		//var productsVm = await _mediator.Send(new GetProductsQuery(userId));
		//return Ok(productsVm);
		await _mediator.Send(new AddProductToOrderCommand(customerId.ToString(), productId.ToString()));
		return Ok();
	}
}
