using DynamicPriceCore.Models;
using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class RunPriceReducingCommand : IRequest<bool>
{
	public string UserId { get; set; }
	public RunPriceReducingCommand(string userId) => UserId = userId;
}
