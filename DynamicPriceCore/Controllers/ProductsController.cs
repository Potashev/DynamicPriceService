using Microsoft.AspNetCore.Mvc;
using MediatR;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.MediatR.ProductEntity.Commands;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace DynamicPriceCore.Controllers
{
	[Route("api/company/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IMediator _mediator;
		public ProductsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProducts(CancellationToken cancellationToken)
		{
			var productsVm = await _mediator.Send(new GetProductsQuery(), cancellationToken);
			return Ok(productsVm);
		}

		[HttpGet("{id}")]
		//todo: add authrize
		public async Task<ActionResult<ProductViewModel>> GetProduct(int id)
		{
			var productVm = await _mediator.Send(new GetProductDetailsQuery((int)id));

			return productVm == null ?
				NotFound() :
				productVm;
		}

		[HttpPut("{id}")]
		//todo: add authrize
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
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
		public async Task<ActionResult<int>> Create(ProductViewModel productVm)
		{
			var productId = await _mediator.Send(new CreateProductCommand(productVm));
			return Ok(productId);
		}

		[HttpDelete("{id}")]
		//todo: add authrize
		public async Task<IActionResult> Delete(int id)
		{
			await _mediator.Send(new DeleteProductCommand(id));
			return Ok();
		}
	}
}
