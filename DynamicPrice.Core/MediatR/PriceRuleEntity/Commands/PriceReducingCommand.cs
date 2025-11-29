using MediatR;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Commands;

public record PriceReducingCommand(bool IsRunCommand) : IRequest;
