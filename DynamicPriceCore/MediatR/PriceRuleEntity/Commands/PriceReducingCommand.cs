using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public record PriceReducingCommand(bool IsRunCommand) : IRequest<bool>;
