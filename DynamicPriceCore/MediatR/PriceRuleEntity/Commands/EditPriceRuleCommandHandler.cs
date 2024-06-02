using AutoMapper;
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
    private readonly IMapper _mapper;

    public EditPriceRuleCommandHandler(DynamicPriceCoreContext context, IMapper mapper)
        => (_context, _mapper) = (context, mapper);

    public async Task<int> Handle(EditPriceRuleCommand request, CancellationToken cancellationToken)
	{
        var updatedPriceRuleVm = request.PriceRuleVm;
        var priceRule = await _context.PriceRules
            .FirstOrDefaultAsync(pr => pr.PriceRuleId == updatedPriceRuleVm.PriceRuleId);

        if (priceRule != null) 
        {
            _mapper.Map(updatedPriceRuleVm, priceRule);

            _context.Update(priceRule);
            _context.SaveChanges();
        }

        return priceRule.PriceRuleId;
	}
}
