using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommand : IRequest<int>
{
	public PriceRuleViewModel PriceRuleVm { get; set; }
	public EditPriceRuleCommand(PriceRuleViewModel priceRuleVm) => PriceRuleVm = priceRuleVm;
}
