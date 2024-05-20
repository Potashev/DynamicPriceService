using Microsoft.AspNetCore.Mvc;
using DynamicPriceService.MediatR.ProductEntity.Queries;
using DynamicPriceService.MediatR.ProductEntity.Commands;
using MediatR;
using DynamicPriceService.MediatR.ViewModel;

namespace DynamicPriceService.Controllers;

public class ProductsController : Controller
{
	private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
		_mediator = mediator;
    }

    // GET: Products
    public async Task<IActionResult> Index()
    {
        return View(await _mediator.Send(new GetProductsQuery()));
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var productVm = await _mediator.Send(new GetProductDetailsQuery((int)id));
        return View(productVm);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Products/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Price,MinimumPrice,Quantity,Description")] ProductViewModel productVm)
    {
        if (ModelState.IsValid)
        {
            await _mediator.Send(new CreateProductCommand(productVm));
            return RedirectToAction(nameof(Index));
        }
        return View(productVm);
    }

	// GET: Products/Edit/5
	public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var productVm = await _mediator.Send(new GetProductDetailsQuery((int)id));
        if (productVm == null)
        {
            return NotFound();
        }
        return View(productVm);
    }

    // POST: Products/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ProductId,Title,Price,MinimumPrice,Quantity,Description")] ProductViewModel productVm)
    {
        if (id != productVm.ProductId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _mediator.Send(new EditProductCommand(productVm));
            return RedirectToAction(nameof(Index));
        }
        return View(productVm);
    }

    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var productVm = await _mediator.Send(new GetProductDetailsQuery((int)id));
        if (productVm == null)
        {
            return NotFound();
        }

        return View(productVm);
    }

    // POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return RedirectToAction(nameof(Index));
    }
}
