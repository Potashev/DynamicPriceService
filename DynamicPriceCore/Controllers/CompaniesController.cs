using DynamicPriceCore.MediatR.CompanyEntity.Queries;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceCore.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CompaniesController : ControllerBase
{
	private readonly IMediator _mediator;
	public CompaniesController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet]
	[Route("/api/ActiveCompanies")]
	public async Task<ActionResult<IEnumerable<Company>>> GetActiveCompanies()
	{
		var activeCompanies = await _mediator.Send(new GetActiveCompaniesQuery());
		return Ok(activeCompanies);
	}
}
