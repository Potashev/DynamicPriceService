using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Commands;

public record TopUpBalanceCommand(BalanceViewModel balanceVm) : IRequest;
