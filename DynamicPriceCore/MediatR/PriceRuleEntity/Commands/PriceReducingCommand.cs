using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class PriceReducingCommand : IRequest<bool>
{
	public string UserId { get; set; }
	public bool IsRunCommand { get; set; }
	public PriceReducingCommand(string userId, bool isRunCommand) => (UserId, IsRunCommand) = (userId, isRunCommand);
}
