using AutoMapper;
using DynamicPriceService.Data;
using DynamicPriceService.MediatR.PriceRuleEntity.Commands;
using DynamicPriceService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommandHandler
	: IRequestHandler<EditPriceRuleCommand, int>
{
	private readonly DynamicPriceServiceContext _context;
    private readonly IMapper _mapper;

    public EditPriceRuleCommandHandler(DynamicPriceServiceContext context, IMapper mapper)
        => (_context, _mapper) = (context, mapper);

    public async Task<int> Handle(EditPriceRuleCommand request, CancellationToken cancellationToken)
	{
        var updatedPriceRuleVm = request.PriceRuleVm;
        var priceRule = await _context.PriceRules
            .FirstOrDefaultAsync(pr => pr.PriceRuleId == updatedPriceRuleVm.PriceRuleId);

        if (priceRule != null) 
        {
            
        }

        _context.Update(priceRule);
        _context.SaveChanges();

        return priceRule.PriceRuleId;
	}
}
