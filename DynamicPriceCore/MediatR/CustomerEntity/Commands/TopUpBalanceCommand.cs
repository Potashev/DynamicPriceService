using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.CustomerEntity.Commands;

public record TopUpBalanceCommand(BalanceViewModel balanceVm) : IRequest;
