using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommand : IRequest<int>
{
	public PriceRule PriceRule { get; set; }
	public EditPriceRuleCommand(PriceRule priceRule) => PriceRule = priceRule;
}
