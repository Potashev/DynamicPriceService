using DynamicPriceCore.MediatR.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.AuthEntity.Commands;

public record RegisterCommand(RegisterViewModel registerVm) : IRequest;
