using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.ManagerEntity.Queries;

public record GetManagerInfoQuery : IRequest<ManagerInfoViewModel>;
