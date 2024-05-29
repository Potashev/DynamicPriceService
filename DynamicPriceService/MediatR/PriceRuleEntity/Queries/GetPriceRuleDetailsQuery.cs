using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleDetailsQuery : IRequest<PriceRule>
{
	public Company Company { get; set; }
	public GetPriceRuleDetailsQuery(Company company) => Company = company;
}
