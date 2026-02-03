using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Queries;

public record GetPriceRuleWithStatusQuery() : IRequest<PriceRuleWithStatus>;
