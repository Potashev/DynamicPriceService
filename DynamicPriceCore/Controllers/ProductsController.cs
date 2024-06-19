using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MediatR;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.MediatR.ProductEntity.Commands;

namespace DynamicPriceCore.Controllers
{
    //[ApiController(SuppressModelStateInvalidFilter = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DynamicPriceCoreContext _context; //todo: remove
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

            if (productVm == null)
            {
                return NotFound();
            }

            return productVm;
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

            //_context.Entry(product).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!ProductExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<int>> PostProduct(ProductViewModel productVm)
        {
            //_context.Products.Add(product);
            //await _context.SaveChangesAsync();
            //return CreatedAtAction("GetProduct", new { id = productVm.ProductId }, productVm);



            var productId = await _mediator.Send(new CreateProductCommand(productVm, _userId));
            return Ok(productId);
            //return RedirectToAction(nameof(Index));
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _mediator.Send(new DeleteProductCommand(id));
            return Ok();

            //var product = await _context.Products.FindAsync(id);
            //if (product == null)
            //{
            //    return NotFound();
            //}

            //_context.Products.Remove(product);
            //await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
