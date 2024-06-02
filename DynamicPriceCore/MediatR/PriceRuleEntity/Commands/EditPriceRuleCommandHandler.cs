using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.PriceRuleEntity.Commands;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommandHandler
	: IRequestHandler<EditPriceRuleCommand, int>
{
	private readonly DynamicPriceCoreContext _context;

    public EditPriceRuleCommandHandler(DynamicPriceCoreContext context)
        => _context = context;

	public async Task<int> Handle(EditPriceRuleCommand request, CancellationToken cancellationToken)
	{
        var updatedPriceRule = request.PriceRule;
        var priceRule = await _context.PriceRules
            .FirstOrDefaultAsync(pr => pr.PriceRuleId == updatedPriceRule.PriceRuleId);

        if (priceRule != null) 
        {
            priceRule.Increase = updatedPriceRule.Increase;
            priceRule.Reduction = updatedPriceRule.Reduction;
            priceRule.NoSellTime = updatedPriceRule.NoSellTime;
        }

        _context.Update(priceRule);
        _context.SaveChanges();

        return priceRule.PriceRuleId;
	}
}
