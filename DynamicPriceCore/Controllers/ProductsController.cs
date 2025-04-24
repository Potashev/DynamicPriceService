using Microsoft.AspNetCore.Mvc;
using MediatR;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.MediatR.ProductEntity.Commands;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace DynamicPriceCore.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IMediator _mediator;
		public ProductsController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
		[HttpGet("/api/Products")]
		//[HttpGet("/api/{userId}/Products")]
		public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProducts(/*string userId,*/ CancellationToken cancellationToken)
		{
			var user = HttpContext.User.Identity;	//надо как-то вытягивать company из manager'а, чтчобы уже слать медиатору


			var userId = "1";


			var productsVm = await _mediator.Send(new GetProductsQuery(userId), cancellationToken);
			return Ok(productsVm);
		}

		// GET: api/Products/5
		[HttpGet("{id}")]
		public async Task<ActionResult<ProductViewModel>> GetProduct(int id)
		{
			var productVm = await _mediator.Send(new GetProductDetailsQuery((int)id));

			return productVm == null ?
				NotFound() :
				productVm;
		}

		// PUT: api/Products/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost("{id}/Edit")]
		public async Task<IActionResult> Edit(int id, ProductViewModel productVm)
		{
			if (id != productVm.ProductId)
			{
				return BadRequest();
			}
			var productId = await _mediator.Send(new EditProductCommand(productVm));
			return Ok(productId);
		}

		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost("/api/{userId}/Products")]
		public async Task<ActionResult<int>> Create(ProductViewModel productVm, string userId)
		{
			var productId = await _mediator.Send(new CreateProductCommand(productVm, userId));
			return Ok(productId);
		}

		// DELETE: api/Products/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await _mediator.Send(new DeleteProductCommand(id));
			return Ok();
		}
	}
}
