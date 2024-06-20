using Microsoft.AspNetCore.Mvc;
using MediatR;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.MediatR.ProductEntity.Commands;

namespace DynamicPriceCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        //todo: temp field to pass in mediator - remove later
        private readonly string _userId = "1";

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProduct()
        {
            var productsVm = await _mediator.Send(new GetProductsQuery(_userId));
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductViewModel productVm)
        {
            if (id != productVm.ProductId)
            {
                return BadRequest();
            }
            var productId = await _mediator.Send(new EditProductCommand(productVm));
            return Ok(productId);
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<int>> PostProduct(ProductViewModel productVm)
        {
            var productId = await _mediator.Send(new CreateProductCommand(productVm, _userId));
            return Ok(productId);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _mediator.Send(new DeleteProductCommand(id));
            return Ok();
        }
    }
}
