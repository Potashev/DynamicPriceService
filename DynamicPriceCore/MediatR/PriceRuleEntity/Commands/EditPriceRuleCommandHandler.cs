using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommandHandler
	: IRequestHandler<EditPriceRuleCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly ICurrentUserService _currentUserService;

	public EditPriceRuleCommandHandler(DynamicPriceCoreContext context, IMapper mapper, ICurrentUserService currentUserService)
		=> (_context, _mapper, _currentUserService) = (context, mapper, currentUserService);

	public async Task<int> Handle(EditPriceRuleCommand request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

		var updatedPriceRuleVm = request.PriceRuleVm;
		var priceRule = await _context.PriceRules
			//todo: check
			.FirstOrDefaultAsync(pr => pr.PriceRuleId == updatedPriceRuleVm.PriceRuleId && pr.Company.CompanyId == manager.CompanyId, cancellationToken);

		if (priceRule != null)
		{
			_mapper.Map(updatedPriceRuleVm, priceRule);

			_context.Update(priceRule);
			await _context.SaveChangesAsync(cancellationToken);
		}

		return priceRule.PriceRuleId;
	}
}
