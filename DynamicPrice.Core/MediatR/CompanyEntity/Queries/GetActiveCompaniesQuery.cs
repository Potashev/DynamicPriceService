using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;

namespace DynamicPrice.Core.MediatR.CompanyEntity.Queries;

public record GetActiveCompaniesQuery : IRequest<IEnumerable<CompanyViewModel>>;