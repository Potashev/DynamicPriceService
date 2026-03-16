using DynamicPrice.Shared.Contracts.Requests;
using DynamicPrice.Shared.Contracts.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.AuthEntity.Commands;

public record LoginCommand(LoginRequest loginVm) : IRequest<TokenResponse>;
