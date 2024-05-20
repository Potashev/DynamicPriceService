using AutoMapper;
using DynamicPriceService.Data;
using DynamicPriceService.MediatR.PriceRuleEntity.Commands;
using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommandHandler
	: IRequestHandler<EditPriceRuleCommand>
{
	private readonly DynamicPriceServiceContext _context;
	private readonly IMapper _mapper;

	public EditPriceRuleCommandHandler(DynamicPriceServiceContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task Handle(EditPriceRuleCommand request, CancellationToken cancellationToken)
	{
		var priceRuleVm = request.PriceRuleVm;
		var priceRule = _mapper.Map<PriceRule>(priceRuleVm);

		//Looks dirty
		priceRule.Company =
			(from pr in _context.PriceRule
			 where pr.PriceRuleId == priceRuleVm.PriceRuleId
			 select pr.Company).FirstOrDefault();

		_context.Update(priceRule);
		_context.SaveChanges();
	}
}
