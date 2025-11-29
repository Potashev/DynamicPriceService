using DynamicPrice.Core.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.CompanyEntity.Queries;

public record GetActiveCompaniesQuery : IRequest<IEnumerable<CompanyViewModel>>;