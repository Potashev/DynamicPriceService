using DynamicPriceService.Data;
using DynamicPriceService.MediatR.PriceRuleEntity.Commands;
using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommandHandler
	: IRequestHandler<EditPriceRuleCommand>
{
	private readonly DynamicPriceServiceContext _context;

    public EditPriceRuleCommandHandler(DynamicPriceServiceContext context)
        => _context = context;

	public async Task Handle(EditPriceRuleCommand request, CancellationToken cancellationToken)
	{
		var priceRule = request.PriceRule;

        //Looks dirty
        priceRule.Company =
			(from pr in _context.PriceRule
			 where pr.PriceRuleId == priceRule.PriceRuleId
			 select pr.Company).FirstOrDefault();

		_context.Update(priceRule);
		_context.SaveChanges();
	}
}
