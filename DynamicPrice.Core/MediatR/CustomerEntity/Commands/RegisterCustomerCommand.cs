using DynamicPrice.Shared.Contracts.Requests;
using MediatR;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Commands;

public record RegisterCustomerCommand(RegisterRequest registerVm) : IRequest;
