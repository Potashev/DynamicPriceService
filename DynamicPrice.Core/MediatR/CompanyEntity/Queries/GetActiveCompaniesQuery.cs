using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public record GetActiveCompaniesQuery : IRequest<IEnumerable<CompanyViewModel>>;