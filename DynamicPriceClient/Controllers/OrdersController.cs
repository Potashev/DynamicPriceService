using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace DynamicPriceClient.Controllers;
public class OrdersController : Controller
{
	private readonly string _localhosturl = "https://localhost:7140";
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly JsonSerializerOptions _options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	//todo: temp field to pass in mediator - remove later
	private readonly string _customerId = "1";

	public OrdersController(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public IActionResult Index()
	{
		return View();
	}

	public async Task<IActionResult> AddProduct(int? id)
	{
		var client = _httpClientFactory.CreateClient();
		var url = $"{_localhosturl}/api/Orders/{_customerId}/{id}";
		var response = await client.GetStringAsync(url);
		// todo: if success - notice customer
		// todo: redirect to cart or to company products
		return View();
	}
}
