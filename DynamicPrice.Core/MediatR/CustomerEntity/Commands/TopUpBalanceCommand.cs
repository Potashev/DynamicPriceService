using DynamicPrice.Shared.Contracts.Requests;
using MediatR;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Commands;

public record TopUpBalanceCommand(BalanceRequest balanceVm) : IRequest;
