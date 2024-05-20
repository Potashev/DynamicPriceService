using DynamicPriceService.MediatR.ViewModel;
using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleDetailsQuery : IRequest<PriceRuleViewModel>
{
	public Company Company { get; set; }
	public GetPriceRuleDetailsQuery(Company company) => Company = company;
}
