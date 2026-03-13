using DynamicPrice.Shared.Contracts.Requests;
using MediatR;

namespace DynamicPrice.Core.MediatR.ManagerEntity.Commands;

public record RegisterManagerCommand(RegisterRequest registerVm) : IRequest;
