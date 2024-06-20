using DynamicPriceService.MediatR.ViewModels;
using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommand : IRequest<int>
{
	public PriceRuleViewModel PriceRuleVm { get; set; }
	public EditPriceRuleCommand(PriceRuleViewModel priceRuleVm) => PriceRuleVm = priceRuleVm;
}
