using DynamicPrice.Core.MediatR.ManagerEntity.Queries;
using DynamicPriceCore.MediatR.CustomerEntity.Queries;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.Controllers;
//[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ManagerPolicy")]
public class ManagerController : ControllerBase
{
	private readonly IMediator _mediator;
	public ManagerController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("api/company/info")]
	public async Task<ActionResult<CompanyViewModel>> GetCompanyInfo(CancellationToken cancellationToken)
	{
		var managerInfo = await _mediator.Send(new GetManagerInfoQuery(), cancellationToken);
		return Ok(managerInfo.Company);
	}
}
