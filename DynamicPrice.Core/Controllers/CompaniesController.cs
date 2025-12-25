//using DynamicPrice.Core.MediatR.CompanyEntity.Queries;
//using DynamicPrice.Shared.Contracts.ViewModels;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace DynamicPrice.Core.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//[Authorize(Roles = "Customer")]
//public class CompaniesController : ControllerBase
//{
//	private readonly IMediator _mediator;
//	public CompaniesController(IMediator mediator)
//	{
//		_mediator = mediator;
//	}

//	[HttpGet]
//	public async Task<ActionResult<IEnumerable<CompanyViewModel>>> GetCompanies([FromQuery] string? status, CancellationToken cancellationToken)
//	{
//		if (status == "active")
//		{
//			var activeCompanies = await _mediator.Send(new GetActiveCompaniesQuery(), cancellationToken);
//			return Ok(activeCompanies);
//		}

//		return StatusCode(StatusCodes.Status501NotImplemented, "Retrieving all companies is not implemented yet.");
//	}

//	[HttpGet("{companyId}/products")]
//	public async Task<ActionResult<CompanyProductsInfo>> GetCompanyProducts(string companyId, CancellationToken cancellationToken)
//	{
//		var companyProducts = await _mediator.Send(new GetCompanyProductsQuery(companyId), cancellationToken);
//		return Ok(companyProducts);
//	}
//}
