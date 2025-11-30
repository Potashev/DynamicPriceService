using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Queries;

public record GetPriceRuleWithStatusQuery() : IRequest<PriceRuleWithStatus>;
