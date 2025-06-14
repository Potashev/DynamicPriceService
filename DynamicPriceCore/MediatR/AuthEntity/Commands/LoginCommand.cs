using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.AuthEntity.Commands;

public record LoginCommand(LoginViewModel loginVm) : IRequest<string>;
