using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class PriceReducingCommandHandler
	: IRequestHandler<PriceReducingCommand, bool>
{
	private DynamicPriceCoreContext _context;
	private IActiveCompaniesService _activeCompaniesService;
	private readonly ICurrentUserService _currentUserService;

	public PriceReducingCommandHandler(DynamicPriceCoreContext context, IActiveCompaniesService activeCompaniesService, ICurrentUserService currentUserService)
		=> (_context, _activeCompaniesService, _currentUserService) = (context, activeCompaniesService, currentUserService);

	public async Task<bool> Handle(PriceReducingCommand request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

		//var company = manager.Company;
		var companyId = (int)manager.CompanyId;

		if (request.IsRunCommand)
		{
			if(!_activeCompaniesService.IsActive(companyId))
				_activeCompaniesService.AddRequest(companyId);
		}
		else
			_activeCompaniesService.RemoveRequest(companyId);

		//temp solution to show actual status after request
		Thread.Sleep(1000);

		return _activeCompaniesService.IsActive(companyId);
	}
}
