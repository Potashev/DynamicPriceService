using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Queries;

public record GetPriceRuleDetailsQuery(string UserId) : IRequest<PriceRuleViewModel>;