using DynamicPriceCore.MediatR.CompanyEntity.Queries;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DynamicPriceCore.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CompaniesController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IHubContext<PriceHub> _priceHubContext;
	public CompaniesController(IMediator mediator, IHubContext<PriceHub> priceHubContext)
	{
		_mediator = mediator;
		_priceHubContext = priceHubContext;
	}

	[HttpGet]
	[Route("/api/ActiveCompanies")]
	public async Task<ActionResult<IEnumerable<Company>>> GetActiveCompanies()
	{
		var activeCompanies = await _mediator.Send(new GetActiveCompaniesQuery());
		return Ok(activeCompanies);
	}

	[HttpGet]
	[Route("/api/ActiveCompanies/{companyId}")]
	public async Task<ActionResult<CompanyProductsInfo>> GetCompanyProducts(string companyId)
	{
		var companyProducts = await _mediator.Send(new GetCompanyProductsQuery(companyId));
		return Ok(companyProducts);
	}

	// Метод, вызываемый сторонним сервисом для обновления цены
	[HttpGet("/api/ActiveCompanies/updateprice")]
	//public async Task<IActionResult> UpdatePrice([FromQuery] int productId, [FromQuery] decimal newPrice)
	public async Task<IActionResult> UpdatePrice()
	{
		// Здесь обновите цену продукта в базе данных
		// productService.UpdatePrice(productId, newPrice);

		var productId = 4;

		// Уведомление всех подключенных клиентов об изменении цены через SignalR
		await _priceHubContext.Clients.All.SendAsync("ReceivePriceUpdate", productId, 100);
		//Thread.Sleep(1000);
		//// Уведомление всех подключенных клиентов об изменении цены через SignalR
		//await _priceHubContext.Clients.All.SendAsync("ReceivePriceUpdate", productId, 130);
		//Thread.Sleep(1000);
		//// Уведомление всех подключенных клиентов об изменении цены через SignalR
		//await _priceHubContext.Clients.All.SendAsync("ReceivePriceUpdate", productId, 120);
		//Thread.Sleep(1000);

		return Ok(new { message = "Price updated and sent to clients" });
	}
}
