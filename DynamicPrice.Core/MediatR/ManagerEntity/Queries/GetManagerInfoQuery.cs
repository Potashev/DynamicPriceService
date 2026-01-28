using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.ManagerEntity.Queries;

public record GetManagerInfoQuery : IRequest<ManagerInfoViewModel>;
