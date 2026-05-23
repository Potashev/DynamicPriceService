using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Commands;

public record EditPriceRuleCommand(PriceRuleViewModel PriceRuleVm) : IRequest<Guid>;
