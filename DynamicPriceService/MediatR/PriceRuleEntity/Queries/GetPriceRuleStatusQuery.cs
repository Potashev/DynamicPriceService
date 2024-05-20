using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleStatusQuery : IRequest<bool>
{
	public Company Company { get; set; }
	public GetPriceRuleStatusQuery(Company company) => Company = company;
}
