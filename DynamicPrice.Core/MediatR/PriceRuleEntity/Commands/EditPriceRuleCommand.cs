using DynamicPrice.Core.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Commands;

public record EditPriceRuleCommand(PriceRuleViewModel PriceRuleVm) : IRequest<int>;
