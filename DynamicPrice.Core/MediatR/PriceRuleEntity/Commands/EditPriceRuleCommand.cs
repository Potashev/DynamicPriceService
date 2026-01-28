using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Commands;

public record EditPriceRuleCommand(PriceRuleViewModel PriceRuleVm) : IRequest<int>;
