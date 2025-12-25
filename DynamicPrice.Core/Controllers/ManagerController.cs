//using DynamicPrice.Core.MediatR.ManagerEntity.Queries;
//using DynamicPrice.Shared.Contracts.ViewModels;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace DynamicPrice.Core.Controllers;

////todo: we try to use it befor login - check

//[ApiController]
//[Authorize(Roles = "Manager")]
//public class ManagerController : ControllerBase
//{
//	private readonly IMediator _mediator;
//	public ManagerController(IMediator mediator)
//	{
//		_mediator = mediator;
//	}

//	[HttpGet("api/company/info")]
//	public async Task<ActionResult<CompanyViewModel>> GetCompanyInfo(CancellationToken cancellationToken)
//	{
//		var managerInfo = await _mediator.Send(new GetManagerInfoQuery(), cancellationToken);
//		return Ok(managerInfo.Company);
//	}
//}
