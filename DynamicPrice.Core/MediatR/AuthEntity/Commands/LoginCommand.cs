using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.AuthEntity.Commands;

public record LoginCommand(LoginViewModel loginVm) : IRequest<string>;
