using DynamicPriceService.MediatR.ViewModel;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommand : IRequest
{
    public PriceRuleViewModel PriceRuleVm { get; set; }
    public EditPriceRuleCommand(PriceRuleViewModel priceRuleVm) => PriceRuleVm = priceRuleVm;
}
