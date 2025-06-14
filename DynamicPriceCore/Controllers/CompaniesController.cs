using DynamicPriceCore.MediatR.CompanyEntity.Queries;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceCore.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CustomerPolicy")]
public class CompaniesController : ControllerBase
{
	private readonly IMediator _mediator;
	public CompaniesController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("/api/ActiveCompanies")]
	public async Task<ActionResult<IEnumerable<Company>>> GetActiveCompanies(CancellationToken cancellationToken)
	{
		var activeCompanies = await _mediator.Send(new GetActiveCompaniesQuery(), cancellationToken);
		return Ok(activeCompanies);
	}

	[HttpGet("/api/ActiveCompanies/{companyId}")]
	public async Task<ActionResult<CompanyProductsInfo>> GetCompanyProducts(string companyId, CancellationToken cancellationToken)
	{
		var companyProducts = await _mediator.Send(new GetCompanyProductsQuery(companyId), cancellationToken);
		return Ok(companyProducts);
	}
}
