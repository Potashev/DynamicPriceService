using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class PriceReducingCommandHandler
	: IRequestHandler<PriceReducingCommand, bool>
{
	private IActiveCompaniesService _activeCompaniesService;
	private readonly ICurrentUserService _currentUserService;

	public PriceReducingCommandHandler(IActiveCompaniesService activeCompaniesService, ICurrentUserService currentUserService)
		=> (_activeCompaniesService, _currentUserService) = (activeCompaniesService, currentUserService);

	public async Task<bool> Handle(PriceReducingCommand request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

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
