using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public record PriceReducingCommand(string UserId, bool IsRunCommand) : IRequest<bool>;
