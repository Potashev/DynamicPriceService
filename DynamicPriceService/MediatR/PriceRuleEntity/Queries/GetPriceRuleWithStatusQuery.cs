using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleWithStatusQuery : IRequest<PriceRuleWithStatusDto>
{
	public string UserId { get; set; }
	public GetPriceRuleWithStatusQuery(string userId) => UserId = userId;
}

public class PriceRuleWithStatusDto
{
    public PriceRule PriceRule { get; set; }
    public bool IsActive { get; set; }
    public PriceRuleWithStatusDto(PriceRule priceRule, bool isActive)
    {
        PriceRule = priceRule;
        IsActive = isActive;
    }
}
