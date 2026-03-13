using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.ManagerEntity.Queries;

public record GetCompanyManagersQuery : IRequest<IEnumerable<ManagerInfoViewModel>>;