using DynamicPrice.Shared.Contracts.ViewModels.Requests;
using MediatR;

namespace DynamicPrice.Core.MediatR.AuthEntity.Commands;

public record RegisterCommand(RegisterViewModel registerVm) : IRequest;
