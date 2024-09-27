using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public record EditPriceRuleCommand(PriceRuleViewModel PriceRuleVm) : IRequest<int>;
