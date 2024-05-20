using DynamicPriceService.Models;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Commands;

public class RunPriceReducingCommand : IRequest<bool>
{
	public Company Company { get; set; }
	public RunPriceReducingCommand(Company company) => Company = company;
}
