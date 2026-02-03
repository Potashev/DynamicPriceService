using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommandHandler
	: IRequestHandler<EditPriceRuleCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public EditPriceRuleCommandHandler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<int> Handle(
		EditPriceRuleCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		var updatedPriceRuleVm = request.PriceRuleVm;
		var priceRule = await _context.PriceRules
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
