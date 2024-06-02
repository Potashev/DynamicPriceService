using Microsoft.AspNetCore.Mvc;
using DynamicPriceService.MediatR.ProductEntity.Queries;
using DynamicPriceService.MediatR.ProductEntity.Commands;
using MediatR;
using DynamicPriceService.Models;
using System;
using System.Text.Json;
using System.Text;

namespace DynamicPriceService.Controllers;

public class ProductsController : Controller
{
    private readonly IMediator _mediator;
    //todo: temp field to pass in mediator - remove later
    private readonly string _userId = "1";

    private readonly string _localhosturl = "https://localhost:7140";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public ProductsController(IMediator mediator, IHttpClientFactory httpClientFactory)
    {
        _mediator = mediator;
        _httpClientFactory = httpClientFactory;
    }

    // GET: Products
    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient();
        var url = $"{_localhosturl}/api/Products";
        var response = await client.GetStringAsync(url);

        var products = JsonSerializer.Deserialize<IEnumerable<Product>>(response, _options);

        return View(products);
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetStringAsync($"{_localhosturl}/api/Products/1");

        var product = JsonSerializer.Deserialize<Product>(response, _options);
        return View(product);

        return View(product);
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
    public async Task<IActionResult> Create(Product product)
    {
        ModelState.Remove("Company");
        if (ModelState.IsValid)
        {
            var client = _httpClientFactory.CreateClient();

            var json = JsonSerializer.Serialize(product);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_localhosturl}/api/Products", data);

            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _mediator.Send(new GetProductDetailsQuery((int)id));
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // POST: Products/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.ProductId)
        {
            return NotFound();
        }

        ModelState.Remove("Company");   //todo: why modelsstate doesn't know about lastsell time?
        if (ModelState.IsValid)
        {
            await _mediator.Send(new EditProductCommand(product));
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _mediator.Send(new GetProductDetailsQuery((int)id));
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
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
