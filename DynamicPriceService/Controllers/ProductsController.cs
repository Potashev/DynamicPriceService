using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using DynamicPriceService.ViewModels;
using System.Net.Http.Headers;

namespace DynamicPriceService.Controllers;

public class ProductsController : Controller
{
	//todo: temp field to pass in mediator - remove later
	private readonly string _userId;

	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public ProductsController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
	{
		_httpClientFactory = httpClientFactory;

		//var context = httpContextAccessor.HttpContext;
		//if (context.Request.Cookies.ContainsKey("User"))
		//	_userId = context.Request.Cookies["User"];
		//else
		//	throw new Exception("User not found");
	}

	// GET: Products
	public async Task<IActionResult> Index()
	{
		var client = _httpClientFactory.CreateClient();

		var token = HttpContext.Session.GetString("AuthToken");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// здесь токен получаю, осталось это в запрос встроить

		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		//var response = await client.GetStringAsync($"{_localhosturl}/api/{_userId}/Products", cts.Token);
		var response = await client.GetStringAsync($"{_localhosturl}/api/Products", cts.Token);
		var productsVm = JsonSerializer.Deserialize<IEnumerable<ProductViewModel>>(response, _options);
		return View(productsVm);
	}

	public async Task<IActionResult> Login()
	{
		var users = new List<int> { 1, 2 };
		return View(users);
	}

	// GET: Products/Details/5
	public async Task<IActionResult> Details(int? id)
	{
		if (id == null)
		{
			return NotFound();
		}
		var client = _httpClientFactory.CreateClient();
		var response = await client.GetStringAsync($"{_localhosturl}/api/Products/{id}");
		var productVm = JsonSerializer.Deserialize<ProductViewModel>(response, _options);
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
	public async Task<IActionResult> Create(ProductViewModel productVm)
	{
		if (ModelState.IsValid)
		{
			var client = _httpClientFactory.CreateClient();
			var json = JsonSerializer.Serialize(productVm);
			var data = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await client.PostAsync($"{_localhosturl}/api/{_userId}/Products", data);
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
		var client = _httpClientFactory.CreateClient();
		var response = await client.GetStringAsync($"{_localhosturl}/api/Products/{id}");
		var productVm = JsonSerializer.Deserialize<ProductViewModel>(response, _options);
		return View(productVm);
	}

	// POST: Products/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, ProductViewModel productVm)
	{
		if (ModelState.IsValid)
		{
			var client = _httpClientFactory.CreateClient();
			var json = JsonSerializer.Serialize(productVm);
			var data = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await client.PostAsync($"{_localhosturl}/api/Products/{id}/Edit", data);
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
		var client = _httpClientFactory.CreateClient();
		var response = await client.GetStringAsync($"{_localhosturl}/api/Products/{id}");
		var productVm = JsonSerializer.Deserialize<ProductViewModel>(response, _options);
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
		var client = _httpClientFactory.CreateClient();
		var response = await client.DeleteAsync($"{_localhosturl}/api/Products/{id}");
		return RedirectToAction(nameof(Index));
	}
}
