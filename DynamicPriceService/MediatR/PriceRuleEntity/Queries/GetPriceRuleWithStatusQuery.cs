using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleWithStatusQuery : IRequest<PriceRuleWithStatusDto>
{
	public Company Company { get; set; }
	public GetPriceRuleWithStatusQuery(Company company) => Company = company;
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
