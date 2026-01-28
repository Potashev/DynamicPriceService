using DynamicPrice.Shared.Contracts.ViewModels.Requests;
using MediatR;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Commands;

public record TopUpBalanceCommand(BalanceViewModel balanceVm) : IRequest;
