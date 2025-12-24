using DynamicPrice.Core.MediatR.ProductEntity.Commands;
using DynamicPrice.Core.MediatR.ProductEntity.Queries;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.Controllers;

[ApiController]
[Route("api/company/[controller]")]
[Authorize(Roles = "Manager")]
public class ProductsController : ControllerBase
{
	private readonly IMediator _mediator;

	private readonly IHttpContextAccessor _httpContextAccessor;
	public ProductsController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
	{
		_mediator = mediator;
		_httpContextAccessor = httpContextAccessor;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProducts(CancellationToken cancellationToken)
	{
		var context = _httpContextAccessor.HttpContext;
		var productsVm = await _mediator.Send(new GetProductsQuery(), cancellationToken);
		return Ok(productsVm);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<ProductViewModel>> GetProduct(int id)
	{
		var productVm = await _mediator.Send(new GetProductDetailsQuery(id));
		return Ok(productVm);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Edit(int id, ProductViewModel productVm)
	{
		if (id != productVm.ProductId)
		{
			return BadRequest();
		}
		var productId = await _mediator.Send(new EditProductCommand(productVm));
		return Ok(productId);
	}

	[HttpPost]
	public async Task<ActionResult<int>> Create(ProductViewModel productVm)
	{
		var productId = await _mediator.Send(new CreateProductCommand(productVm));
		return Ok(productId);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		await _mediator.Send(new DeleteProductCommand(id));
		return Ok();
	}
}