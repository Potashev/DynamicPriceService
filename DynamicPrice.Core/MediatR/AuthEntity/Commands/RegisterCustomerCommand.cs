using DynamicPrice.Shared.Contracts.Requests;
using MediatR;

namespace DynamicPrice.Core.MediatR.AuthEntity.Commands;

public record RegisterCustomerCommand(RegisterRequest registerVm) : IRequest;
